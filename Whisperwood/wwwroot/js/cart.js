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
        return response.json();
    } catch (error) {
        console.error(error);
        return [];
    }
};

const updateCartButtonIcon = (button, isInCart) => {
    button.innerHTML = isInCart ? cartRemoveIcon : cartAddIcon;
};

const createQuantityDialog = (isInCart, stock, currentQuantity, onConfirm) => {
    const dialog = document.createElement('dialog');
    dialog.id = 'quantity-dialog';
    dialog.className = 'bg-primary border-accent1 rounded-lg border-[2px] p-6 max-w-sm mx-auto';
    dialog.innerHTML = `
        <h3 class="text-accent3 text-lg font-semibold mb-4">${isInCart ? 'Update Quantity' : 'Select Quantity'}</h3>
        <label for="quantity-input" class="text-accent2 block mb-2">Quantity (1-${stock}):</label>
        <input type="number" id="quantity-input" min="1" max="${stock}" value="${currentQuantity}" class="border-accent2 w-full rounded border p-2 text-gray-800 mb-4" />
        <div class="flex justify-end gap-2">
            <button id="cancel-quantity" class="bg-gray-500 rounded px-3 py-1 text-white hover:bg-gray-600">Cancel</button>
            <button id="confirm-quantity" class="bg-accent3 rounded px-3 py-1 text-white hover:bg-accent2">${isInCart ? 'Update Cart' : 'Add to Cart'}</button>
        </div>
    `;
    document.body.appendChild(dialog);
    dialog.showModal();

    dialog.querySelector('#cancel-quantity').addEventListener('click', () => dialog.close());
    dialog.querySelector('#confirm-quantity').addEventListener('click', onConfirm);

    return dialog;
};

const handleCartAction = async (button, bookId, stock, isInCart, currentQuantity) => {
    const dialog = createQuantityDialog(isInCart, stock, currentQuantity, async () => {
        const quantity = parseInt(document.getElementById('quantity-input').value);
        if (isNaN(quantity) || quantity < 1 || quantity > stock) {
            alert(`Please enter a valid quantity between 1 and ${stock}.`);
            return;
        }

        try {
            const endpoint = isInCart ? 'https://localhost:7018/api/CartItem/update' : 'https://localhost:7018/api/CartItem/add';
            const method = isInCart ? 'PUT' : 'POST';
            const response = await fetch(endpoint, {
                method,
                headers: {
                    'Authorization': `Bearer ${window.jwtToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ bookId, quantity })
            });

            const data = await response.json();

            if (response.ok) {
                alert(isInCart ? 'Cart updated successfully!' : 'Book added to cart successfully!');
                dialog.close();
                updateCartButton(button, bookId);
            } else {
                alert(data.message || `Failed to ${isInCart ? 'update' : 'add to'} cart.`);
            }
        } catch (error) {
            alert(`An error occurred while ${isInCart ? 'updating' : 'adding to'} the cart.`);
        }
    });
};

const updateCartButton = async (button, bookId) => {
    if (!window.isAuthenticated) {
        button.innerHTML = cartAddIcon;
        return;
    }

    const cartItems = await fetchCartItems();
    const cartItem = cartItems.find(item => item.bookId === bookId);
    const isInCart = !!cartItem;
    updateCartButtonIcon(button, isInCart);
    return cartItem;
};

const initializeCartButton = async (buttonId, bookId, stock) => {
    const button = document.getElementById(buttonId);
    if (!button) return;

    await updateCartButton(button, bookId);

    button.addEventListener('click', async (e) => {
        e.preventDefault();
        if (!window.isAuthenticated) {
            window.location.href = '/User/Login';
            return;
        }

        const cartItem = await updateCartButton(button, bookId);
        const isInCart = !!cartItem;
        const currentQuantity = cartItem ? cartItem.quantity : 1;

        handleCartAction(button, bookId, stock, isInCart, currentQuantity);
    });
};

// Attach to window to make it globally available
window.initializeCartButton = initializeCartButton;
