async function initializeWishlistButton(buttonId, bookId) {
    const button = document.getElementById(buttonId);

    async function updateWishlistButton() {
        if (!window.isAuthenticated) {
            button.textContent = 'Add to Wishlist';
            return;
        }

        try {
            const response = await fetch('https://localhost:7018/api/WishlistItem/getall', {
                headers: { 'Authorization': `Bearer ${window.jwtToken}` }
            });
            if (!response.ok) throw new Error('Failed to fetch wishlist.');
            const wishlistItems = await response.json();
            button.textContent = wishlistItems.some(item => item.bookId === bookId)
                ? 'Remove from Wishlist'
                : 'Add to Wishlist';
        } catch (error) {
            console.error('Error checking wishlist:', error);
            button.textContent = 'Add to Wishlist';
        }
    }

    async function handleWishlist() {
        if (!window.isAuthenticated) {
            window.location.href = '/User/Login';
            return;
        }

        try {
            const checkResponse = await fetch('https://localhost:7018/api/WishlistItem/getall', {
                headers: { 'Authorization': `Bearer ${window.jwtToken}` }
            });
            if (!checkResponse.ok) throw new Error('Failed to fetch wishlist.');
            const wishlistItems = await checkResponse.json();
            const isInWishlist = wishlistItems.some(item => item.bookId === bookId);

            if (isInWishlist) {
                const deleteResponse = await fetch(`https://localhost:7018/api/WishlistItem/delete/${bookId}`, {
                    method: 'DELETE',
                    headers: { 'Authorization': `Bearer ${window.jwtToken}` }
                });
                if (!deleteResponse.ok) throw new Error('Failed to remove from wishlist.');
                alert(`Removed from wishlist!`);
            } else {
                const addResponse = await fetch('https://localhost:7018/api/WishlistItem/add', {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${window.jwtToken}`,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ bookId })
                });
                if (!addResponse.ok) throw new Error('Failed to add to wishlist.');
                alert(`Added to wishlist!`);
            }
            await updateWishlistButton();
        } catch (error) {
            console.error('Wishlist error:', error);
            alert('An error occurred while updating the wishlist. Please try again.');
        }
    }

    await updateWishlistButton();
    button.addEventListener('click', (e) => {
        e.preventDefault();
        handleWishlist();
    });
}

// Attach to window to make it globally available
window.initializeWishlistButton = initializeWishlistButton;