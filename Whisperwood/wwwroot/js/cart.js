const cartAddIcon = `
    <svg width="24" height="24" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" stroke="#fff" stroke-width="2">
        <path fill="none" stroke-linecap="round" d="M11 20.5h.1m5.9 0h.1M3 3h2.14a1 1 0 0 1 1 .85L6.62 7 8 16l11-1 2-8H6.62"/>
    </svg>
`;

const cartRemoveIcon = `
    <svg width="24" height="24" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" stroke-linecap="round">
        <path fill="#fff" d="m21 7-2 8-11 1-1.38-9z"/>
        <path fill="none" stroke="#fff" stroke-width="2" stroke-linejoin="round" d="M11 20.5h.1m5.9 0h.1M3 3h2.14a1 1 0 0 1 1 .85L6.62 7 8 16l11-1 2-8H6.62"/>
    </svg>
`;

// calculates book price with active discount
const calculateBookPrice = (book) => {
    if (book.isOnSale && book.discountPercentage > 0) {
        const today = new Date();
        const startDate = new Date(book.discountStartDate ?? '');
        const endDate = new Date(book.discountEndDate ?? '');
        if (startDate <= today && endDate >= today && !isNaN(startDate) && !isNaN(endDate)) {
            return book.price * (1 - book.discountPercentage / 100);
        }
    }
    return book.price;
};

// fetches cart items from api
const fetchCartItems = async () => {
    if (!window.jwtToken) return [];

    try {
        const response = await fetch('https://localhost:7018/api/CartItem/getall', {
            headers: { 'Authorization': `Bearer ${window.jwtToken}` }
        });

        if (response.status === 401) {
            Toast.fire({
                icon: 'error',
                title: 'Please log in to manage cart'
            }).then(() => {
                window.location.href = '/User/Login';
            });
            return [];
        }

        if (!response.ok) throw new Error('Failed to fetch cart');

        return await response.json();
    } catch (error) {
        Toast.fire({
            icon: 'error',
            title: error.message || 'Failed to fetch cart'
        });
        return [];
    }
};

// updates cart button icon for book list pages
const updateCartButtonIcon = (button, isInCart) => {
    button.innerHTML = isInCart ? cartRemoveIcon : cartAddIcon;
};

// creates quantity dialog for adding to cart on book list pages
const createQuantityDialog = (book, onConfirm) => {
    const effectivePrice = calculateBookPrice(book);
    const isOnSale = effectivePrice < book.price;
    const dialog = document.createElement('dialog');
    dialog.id = 'quantity-dialog';
    dialog.className = 'bg-primary border-accent1 rounded border-[2px] p-6 max-w-sm mx-auto';
    dialog.innerHTML = `
        <h3 class="text-accent3 text-lg font-semibold mb-4">Add "${book.title}" to Cart</h3>
        <p class="text-accent2 mb-2">Price: ${isOnSale ? `<span class="text-accent4">Rs. ${effectivePrice.toFixed(2)}</span> <span class="line-through text-accent1">Rs. ${book.price.toFixed(2)}</span>` : `Rs. ${book.price.toFixed(2)}`}</p>
        <p class="text-accent2 mb-2">Stock: ${book.availabilityStatus && book.stock > 0 ? `In Stock (${book.stock})` : 'Out of Stock'}</p>
        <label for="quantity-input" class="text-accent2 block mb-2">Quantity (1-${book.stock}):</label>
        <input type="number" id="quantity-input" min="1" max="${book.stock}" value="1" class="border-accent2 w-full rounded border p-2 text-gray-800 mb-4" ${book.stock <= 0 ? 'disabled' : ''} />
        <div class="flex justify-end gap-2">
            <button id="cancel-quantity" class="bg-gray-500 rounded px-3 py-1 text-white hover:bg-gray-600">Cancel</button>
            <button id="confirm-quantity" class="bg-accent3 rounded px-3 py-1 text-white hover:bg-accent2" ${book.stock <= 0 ? 'disabled' : ''}>Add to Cart</button>
        </div>
    `;
    document.body.appendChild(dialog);
    dialog.showModal();

    const elements = {
        cancel: dialog.querySelector('#cancel-quantity'),
        confirm: dialog.querySelector('#confirm-quantity')
    };

    elements.cancel.addEventListener('click', () => dialog.close());
    if (book.stock > 0) {
        elements.confirm.addEventListener('click', onConfirm);
    }

    return dialog;
};

// handles add or remove cart actions for book list pages
const handleCartAction = async (button, bookId, book, isInCart) => {
    if (!window.checkAuth('manage cart')) return;

    if (isInCart) {
        const result = await Swal.fire({
            title: 'Remove from Cart',
            text: `Are you sure you want to remove "${book.title}" from your cart?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#E74C3C',
            cancelButtonColor: '#2C3E50',
            confirmButtonText: 'Remove',
            cancelButtonText: 'Cancel'
        });

        if (!result.isConfirmed) return;

        try {
            const response = await fetch(`https://localhost:7018/api/CartItem/delete/${bookId}`, {
                method: 'DELETE',
                headers: { 'Authorization': `Bearer ${window.jwtToken}` }
            });

            if (response.status === 401) {
                Toast.fire({
                    icon: 'error',
                    title: 'Please log in to manage cart'
                }).then(() => {
                    window.location.href = '/User/Login';
                });
                return;
            }

            if (!response.ok) throw new Error('Failed to remove book from cart');

            Toast.fire({
                icon: 'success',
                title: 'Book removed from cart'
            }).then(() => {
                updateCartButton(button, bookId, book);
                window.location.reload();
            });
        } catch (error) {
            Toast.fire({
                icon: 'error',
                title: error.message || 'Failed to remove book from cart'
            });
        }
    } else {
        const pubDate = new Date(book.publicationDate);
        const now = new Date();
        if (!isNaN(pubDate) && pubDate > now) {
            Toast.fire({
                icon: 'error',
                title: 'This book is not yet available for purchase'
            });
            return;
        }
        if (!book.availabilityStatus || book.stock <= 0) {
            Toast.fire({
                icon: 'error',
                title: 'This book is out of stock'
            });
            return;
        }

        const dialog = createQuantityDialog(book, async () => {
            const quantity = parseInt(document.getElementById('quantity-input').value);
            if (isNaN(quantity) || quantity < 1 || quantity > book.stock) {
                Toast.fire({
                    icon: 'error',
                    title: `Please enter a valid quantity between 1 and ${book.stock}`
                });
                return;
            }

            try {
                const response = await fetch('https://localhost:7018/api/CartItem/add', {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${window.jwtToken}`,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ bookId, quantity })
                });

                if (response.status === 401) {
                    Toast.fire({
                        icon: 'error',
                        title: 'Please log in to manage cart'
                    }).then(() => {
                        window.location.href = '/User/Login';
                    });
                    return;
                }

                if (!response.ok) throw new Error('Failed to add book to cart');

                Toast.fire({
                    icon: 'success',
                    title: `Book added to cart for Rs. ${(quantity * calculateBookPrice(book)).toFixed(2)}`
                }).then(() => {
                    dialog.close();
                    updateCartButton(button, bookId, book);
                    window.location.reload();
                });
            } catch (error) {
                Toast.fire({
                    icon: 'error',
                    title: error.message || 'Failed to add book to cart'
                });
            }
        });
    }
};

// updates cart button state for book list pages
const updateCartButton = async (button, bookId, book) => {
    if (!window.jwtToken) {
        updateCartButtonIcon(button, false);
        button.disabled = true;
        button.classList.add('opacity-50', 'cursor-not-allowed');
        return null;
    }

    const cartItems = await fetchCartItems();
    const isInCart = cartItems.some(item => item.bookId === bookId);
    updateCartButtonIcon(button, isInCart);
    button.disabled = book.stock <= 0 || !book.availabilityStatus;
    button.classList.remove('opacity-50', 'cursor-not-allowed');
    button.setAttribute('data-price', calculateBookPrice(book).toFixed(2));
    button.setAttribute('data-stock', book.stock);
    button.setAttribute('data-availability', book.availabilityStatus ? 'In Stock' : 'Out of Stock');
    return isInCart ? cartItems.find(item => item.bookId === bookId) : null;
};

// updates cart item quantity
const updateCartItem = async (bookId, quantity, successCallback, errorCallback) => {
    if (!window.checkAuth('manage cart')) return;

    if (isNaN(quantity) || quantity < 1) {
        errorCallback('Quantity must be at least 1');
        return;
    }

    try {
        const response = await fetch('https://localhost:7018/api/CartItem/update', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${window.jwtToken}`
            },
            body: JSON.stringify({ bookId, quantity })
        });

        if (response.status === 401) {
            Toast.fire({
                icon: 'error',
                title: 'Please log in to manage cart'
            }).then(() => {
                window.location.href = '/User/Login';
            });
            return;
        }

        if (!response.ok) throw new Error('Failed to update cart item');

        successCallback('Cart item updated successfully');
    } catch (error) {
        errorCallback(error.message || 'Failed to update cart item');
    }
};

// deletes cart item
const deleteCartItem = async (bookId, successCallback, errorCallback) => {
    if (!window.checkAuth('manage cart')) return;

    const result = await Swal.fire({
        title: 'Are you sure you want to remove this book from your cart?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: 'red',
        cancelButtonColor: 'grey',
        confirmButtonText: 'Delete'
    });

    if (!result.isConfirmed) return;

    try {
        const response = await fetch(`https://localhost:7018/api/CartItem/delete/${bookId}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${window.jwtToken}` }
        });

        if (response.status === 401) {
            Toast.fire({
                icon: 'error',
                title: 'Please log in to manage cart'
            }).then(() => {
                window.location.href = '/User/Login';
            });
            return;
        }

        if (!response.ok) throw new Error('Failed to remove book from cart');

        successCallback('Book removed from cart successfully');
    } catch (error) {
        errorCallback(error.message || 'Failed to remove book from cart');
    }
};

// initializes cart button for a book
const initializeCartButton = async (buttonId, bookId, book) => {
    const button = document.getElementById(buttonId);

    // handle book list page add/remove buttons
    if (!buttonId.startsWith('update-') && !buttonId.startsWith('delete-')) {
        await updateCartButton(button, bookId, book);
        button.addEventListener('click', async (e) => {
            e.preventDefault();
            if (!window.checkAuth('manage cart')) return;
            const cartItem = await updateCartButton(button, bookId, book);
            const isInCart = !!cartItem;
            await handleCartAction(button, bookId, book, isInCart);
        });
        return;
    }

    // handle cart page update/delete buttons
    button.disabled = book.stock <= 0 || !book.availabilityStatus;
    button.setAttribute('data-price', calculateBookPrice(book).toFixed(2));
    button.setAttribute('data-stock', book.stock);
    button.setAttribute('data-availability', book.availabilityStatus ? 'In Stock' : 'Out of Stock');

    button.addEventListener('click', async (e) => {
        e.preventDefault();
        if (!window.checkAuth('manage cart')) return;

        if (buttonId.startsWith('update-')) {
            const quantityInput = document.getElementById(`quantity-${bookId}`);
            const quantity = parseInt(quantityInput.value);
            await updateCartItem(
                bookId,
                quantity,
                (message) => {
                    Toast.fire({
                        icon: 'success',
                        title: message
                    }).then(() => {
                        window.location.reload();
                    });
                },
                (error) => {
                    Toast.fire({
                        icon: 'error',
                        title: error
                    });
                }
            );
        } else if (buttonId.startsWith('delete-')) {
            await deleteCartItem(
                bookId,
                (message) => {
                    Toast.fire({
                        icon: 'success',
                        title: message
                    }).then(() => {
                        window.location.reload();
                    });
                },
                (error) => {
                    Toast.fire({
                        icon: 'error',
                        title: error
                    });
                }
            );
        }
    });
};

// exposes functions globally
window.calculateBookPrice = calculateBookPrice;
window.fetchCartItems = fetchCartItems;
window.initializeCartButton = initializeCartButton;
window.updateCartItem = updateCartItem;
window.deleteCartItem = deleteCartItem;