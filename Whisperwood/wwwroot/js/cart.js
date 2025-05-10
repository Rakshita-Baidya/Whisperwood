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

const calculateBookPrice = (book) => {
    if (book.isOnSale && book.discountPercentage > 0) {
        const today = new Date();
        const startDate = book.discountStartDate ? new Date(book.discountStartDate) : null;
        const endDate = book.discountEndDate ? new Date(book.discountEndDate) : null;
        const isSaleActive = startDate && endDate && !isNaN(startDate) && !isNaN(endDate) &&
            startDate <= today && endDate >= today;
        if (isSaleActive) {
            return book.price * (1 - book.discountPercentage / 100);
        }
    }
    return book.price;
};

const fetchCartItems = async () => {
    try {
        const response = await fetch('https://localhost:7018/api/CartItem/getall', {
            headers: { 'Authorization': `Bearer ${window.jwtToken}` }
        });
        if (!response.ok) throw new Error('Failed to fetch cart.');
        return await response.json();
    } catch (error) {
        console.error('Error fetching cart items:', error);
        return [];
    }
};

const updateCartButtonIcon = (button, isInCart) => {
    button.innerHTML = isInCart ? cartRemoveIcon : cartAddIcon;
};

const createQuantityDialog = (book, onConfirm) => {
    const effectivePrice = window.calculateBookPrice(book);
    const isOnSale = effectivePrice < book.price;
    const dialog = document.createElement('dialog');
    dialog.id = 'quantity-dialog';
    dialog.className = 'bg-primary border-accent1 rounded-lg border-[2px] p-6 max-w-sm mx-auto';
    dialog.innerHTML = `
        <h3 class="text-accent3 text-lg font-semibold mb-4">Add "${book.title}" to Cart</h3>
        <p class="text-accent2 mb-2">Price: ${isOnSale ? `<span class="text-red-500">Rs. ${effectivePrice.toFixed(2)}</span> <span class="line-through text-accent1">Rs. ${book.price.toFixed(2)}</span>` : `Rs. ${book.price.toFixed(2)}`}</p>
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

    dialog.querySelector('#cancel-quantity').addEventListener('click', () => dialog.close());
    if (book.stock > 0) {
        dialog.querySelector('#confirm-quantity').addEventListener('click', onConfirm);
    }

    return dialog;
};

const handleCartAction = async (button, bookId, book, isInCart) => {
    if (isInCart) {
        if (confirm('Are you sure you want to remove this book from your cart?')) {
            try {
                const response = await fetch(`https://localhost:7018/api/CartItem/delete/${bookId}`, {
                    method: 'DELETE',
                    headers: {
                        'Authorization': `Bearer ${window.jwtToken}`
                    }
                });

                const data = await response.json();

                if (response.ok) {
                    alert('Book removed from cart successfully!');
                    await updateCartButton(button, bookId, book);
                    if (typeof renderBooks === 'function') {
                        renderBooks(currentPage);
                    }
                } else {
                    alert(data.message || 'Failed to remove book from cart.');
                }
            } catch (error) {
                console.error('Error removing book from cart:', error);
                alert('An error occurred while removing the book from the cart.');
            }
        }
    } else {
        if (!book.availabilityStatus || book.stock <= 0) {
            alert('This book is out of stock.');
            return;
        }
        const dialog = createQuantityDialog(book, async () => {
            const quantity = parseInt(document.getElementById('quantity-input').value);
            if (isNaN(quantity) || quantity < 1 || quantity > book.stock) {
                alert(`Please enter a valid quantity between 1 and ${book.stock}.`);
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

                const data = await response.json();

                if (response.ok) {
                    alert(`Book added to cart for Rs. ${(quantity * window.calculateBookPrice(book)).toFixed(2)}!`);
                    dialog.close();
                    await updateCartButton(button, bookId, book);
                    if (typeof renderBooks === 'function') {
                        renderBooks(currentPage);
                    }
                } else {
                    alert(data.message || 'Failed to add book to cart.');
                }
            } catch (error) {
                console.error('Error adding book to cart:', error);
                alert('An error occurred while adding the book to the cart.');
            }
        });
    }
};

const updateCartButton = async (button, bookId, book) => {
    if (!window.isAuthenticated) {
        button.innerHTML = cartAddIcon;
        button.disabled = book.stock <= 0 || !book.availabilityStatus;
        return null;
    }

    const cartItems = await fetchCartItems();
    const cartItem = cartItems.find(item => item.bookId === bookId);
    const isInCart = !!cartItem;
    updateCartButtonIcon(button, isInCart);
    button.disabled = book.stock <= 0 || !book.availabilityStatus;
    button.setAttribute('data-price', window.calculateBookPrice(book).toFixed(2));
    button.setAttribute('data-stock', book.stock);
    button.setAttribute('data-availability', book.availabilityStatus ? 'In Stock' : 'Out of Stock');
    return cartItem;
};

const updateCartItem = async (bookId, quantity, successCallback, errorCallback) => {
    if (isNaN(quantity) || quantity < 1) {
        errorCallback('Quantity must be at least 1.');
        return;
    }

    try {
        const response = await fetch('https://localhost:7018/api/CartItem/update', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${window.jwtToken || ''}`
            },
            body: JSON.stringify({ bookId, quantity })
        });

        if (!response.ok) {
            if (response.status === 401) {
                window.location.href = '/User/Login';
                return;
            }
            const data = await response.json();
            throw new Error(data.message || 'Failed to update cart item.');
        }

        successCallback('Cart item updated successfully!', true);
    } catch (error) {
        console.error('Error updating cart item:', error);
        errorCallback(error.message);
    }
};

const deleteCartItem = async (bookId, successCallback, errorCallback) => {
    if (!confirm('Are you sure you want to remove this book from your cart?')) return;

    try {
        const response = await fetch(`https://localhost:7018/api/CartItem/delete/${bookId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${window.jwtToken || ''}`
            }
        });

        if (!response.ok) {
            if (response.status === 401) {
                window.location.href = '/User/Login';
                return;
            }
            const data = await response.json();
            throw new Error(data.message || 'Failed to remove book from cart.');
        }

        successCallback('Book removed from cart successfully!', true);
    } catch (error) {
        console.error('Error removing cart item:', error);
        errorCallback(error.message);
    }
};

const initializeCartButton = async (buttonId, bookId, book) => {
    const button = document.getElementById(buttonId);
    if (!button) {
        console.error(`Button not found: ${buttonId}`);
        return;
    }

    await updateCartButton(button, bookId, book);

    button.addEventListener('click', async (e) => {
        e.preventDefault();
        if (!window.isAuthenticated) {
            window.location.href = '/User/Login';
            return;
        }

        const cartItem = await updateCartButton(button, bookId, book);
        const isInCart = !!cartItem;

        console.log(`Cart button clicked for bookId: ${bookId}, isInCart: ${isInCart}`);
        handleCartAction(button, bookId, book, isInCart);
    });
};

// Expose functions to global scope
window.calculateBookPrice = calculateBookPrice;
window.initializeCartButton = initializeCartButton;
window.updateCartItem = updateCartItem;
window.deleteCartItem = deleteCartItem;