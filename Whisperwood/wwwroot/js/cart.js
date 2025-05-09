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

const createQuantityDialog = (stock, onConfirm) => {
    const dialog = document.createElement('dialog');
    dialog.id = 'quantity-dialog';
    dialog.className = 'bg-primary border-accent1 rounded-lg border-[2px] p-6 max-w-sm mx-auto';
    dialog.innerHTML = `
        <h3 class="text-accent3 text-lg font-semibold mb-4">Select Quantity</h3>
        <label for="quantity-input" class="text-accent2 block mb-2">Quantity (1-${stock}):</label>
        <input type="number" id="quantity-input" min="1" max="${stock}" value="1" class="border-accent2 w-full rounded border p-2 text-gray-800 mb-4" />
        <div class="flex justify-end gap-2">
            <button id="cancel-quantity" class="bg-gray-500 rounded px-3 py-1 text-white hover:bg-gray-600">Cancel</button>
            <button id="confirm-quantity" class="bg-accent3 rounded px-3 py-1 text-white hover:bg-accent2">Add to Cart</button>
        </div>
    `;
    document.body.appendChild(dialog);
    dialog.showModal();

    dialog.querySelector('#cancel-quantity').addEventListener('click', () => dialog.close());
    dialog.querySelector('#confirm-quantity').addEventListener('click', onConfirm);

    return dialog;
};

const handleCartAction = async (button, bookId, stock, isInCart) => {
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
                    await updateCartButton(button, bookId);
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
        const dialog = createQuantityDialog(stock, async () => {
            const quantity = parseInt(document.getElementById('quantity-input').value);
            if (isNaN(quantity) || quantity < 1 || quantity > stock) {
                alert(`Please enter a valid quantity between 1 and ${stock}.`);
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
                    alert('Book added to cart successfully!');
                    dialog.close();
                    await updateCartButton(button, bookId);
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

const updateCartButton = async (button, bookId) => {
    if (!window.isAuthenticated) {
        button.innerHTML = cartAddIcon;
        return null;
    }

    const cartItems = await fetchCartItems();
    const cartItem = cartItems.find(item => item.bookId === bookId);
    const isInCart = !!cartItem;
    updateCartButtonIcon(button, isInCart);
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
        console.error('Error removing book from cart:', error);
        errorCallback(error.message);
    }
};

const initializeCartButton = async (buttonId, bookId, stock) => {
    const button = document.getElementById(buttonId);
    if (!button) {
        console.error(`Button not found: ${buttonId}`);
        return;
    }

    await updateCartButton(button, bookId);

    button.addEventListener('click', async (e) => {
        e.preventDefault();
        if (!window.isAuthenticated) {
            window.location.href = '/User/Login';
            return;
        }

        const cartItem = await updateCartButton(button, bookId);
        const isInCart = !!cartItem;

        console.log(`Cart button clicked for bookId: ${bookId}, isInCart: ${isInCart}`);
        handleCartAction(button, bookId, stock, isInCart);
    });
};

window.initializeCartButton = initializeCartButton;