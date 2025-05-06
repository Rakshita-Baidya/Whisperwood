const initializeCartButton = (buttonId, bookId, stock) => {
    const button = document.getElementById(buttonId);
    if (!button) return;

    async function updateCartButton() {
        if (!window.isAuthenticated) {
            button.textContent = 'Add to Cart';
            return;
        }

        try {
            const response = await fetch('https://localhost:7018/api/CartItem/getall', {
                headers: { 'Authorization': `Bearer ${window.jwtToken}` }
            });
            if (!response.ok) throw new Error('Failed to fetch cart.');
            const cartItems = await response.json();
            const cartItem = cartItems.find(item => item.bookId === bookId);
            button.textContent = cartItem ? 'Already in Cart' : 'Add to Cart';
            return cartItem; // Return cart item for quantity info
        } catch (error) {
            console.error('Error checking cart:', error);
            button.textContent = 'Add to Cart';
            return null;
        }
    }

    async function handleCartAction() {
        if (!window.isAuthenticated) {
            window.location.href = '/User/Login';
            return;
        }

        const cartItem = await updateCartButton();
        const isInCart = !!cartItem;
        const currentQuantity = cartItem ? cartItem.quantity : 1;

        // Create or get dialog element
        let dialog = document.getElementById('quantity-dialog');
        if (!dialog) {
            dialog = document.createElement('dialog');
            dialog.id = 'quantity-dialog';
            dialog.className = 'bg-primary border-accent1 rounded-lg border-[2px] p-6 max-w-sm mx-auto';
            document.body.appendChild(dialog);
        }

        // Update dialog content
        dialog.innerHTML = `
            <h3 class="text-accent3 text-lg font-semibold mb-4">${isInCart ? 'Update Quantity' : 'Select Quantity'}</h3>
            <label for="quantity-input" class="text-accent2 block mb-2">Quantity (1-${stock}):</label>
            <input type="number" id="quantity-input" min="1" max="${stock}" value="${currentQuantity}" class="border-accent2 w-full rounded border p-2 text-gray-800 mb-4" />
            <div class="flex justify-end gap-2">
                <button id="cancel-quantity" class="bg-gray-500 rounded px-3 py-1 text-white hover:bg-gray-600">Cancel</button>
                <button id="confirm-quantity" class="bg-accent3 rounded px-3 py-1 text-white hover:bg-accent2">${isInCart ? 'Update Cart' : 'Add to Cart'}</button>
            </div>
        `;

        // Show dialog
        dialog.showModal();

        // Handle cancel button
        document.getElementById('cancel-quantity').addEventListener('click', () => {
            dialog.close();
        });

        // Handle confirm button
        document.getElementById('confirm-quantity').onclick = async () => {
            const quantity = parseInt(document.getElementById('quantity-input').value);
            if (isNaN(quantity) || quantity < 1 || quantity > stock) {
                alert(`Please enter a valid quantity between 1 and ${stock}.`);
                return;
            }

            try {
                const endpoint = isInCart ? 'https://localhost:7018/api/CartItem/update' : 'https://localhost:7018/api/CartItem/add';
                const response = await fetch(endpoint, {
                    method: isInCart ? 'PUT' : 'POST',
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
                    await updateCartButton(); // Refresh button state
                } else {
                    alert(data.message || `Failed to ${isInCart ? 'update' : 'add to'} cart.`);
                }
            } catch (error) {
                alert(`An error occurred while ${isInCart ? 'updating' : 'adding to'} the cart.`);
            }
        };
    }

    // Initial button state update
    updateCartButton();

    button.addEventListener('click', (e) => {
        e.preventDefault();
        handleCartAction();
    });
};

// Attach to window to make it globally available
window.initializeCartButton = initializeCartButton;