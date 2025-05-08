const wishlistIcons = {
    add: `
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32" height="24" width="24">
            <path stroke-width="3" fill="none" stroke="#fff" d="M21.3 28.3 16 23l-5.3 5.3c-.6.6-1.7.2-1.7-.7V5c0-.6.4-1 1-1h12c.6 0 1 .4 1 1v22.6c0 .9-1.1 1.3-1.7.7"/>
        </svg>
    `,
    remove: `
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32" height="24" width="24">
            <path fill="#fff" d="M22 3H10c-1.1 0-2 .9-2 2v22.6c0 .8.5 1.5 1.2 1.8.8.3 1.6.1 2.2-.4l4.6-4.6 4.6 4.6c.4.4.9.6 1.4.6.3 0 .5 0 .8-.2.8-.3 1.2-1 1.2-1.8V5c0-1.1-.9-2-2-2"/>
        </svg>
    `
};

async function fetchWishlist() {
    const response = await fetch('https://localhost:7018/api/WishlistItem/getall', {
        headers: { 'Authorization': `Bearer ${window.jwtToken}` }
    });
    if (!response.ok) throw new Error('Failed to fetch wishlist.');
    return response.json();
}

function updateButtonIcon(button, isInWishlist) {
    button.innerHTML = isInWishlist ? wishlistIcons.remove : wishlistIcons.add;
}

async function initializeWishlistButton(buttonId, bookId) {
    const button = document.getElementById(buttonId);
    if (!button) return;

    async function refreshIcon() {
        try {
            if (!window.isAuthenticated) {
                updateButtonIcon(button, false);
                return;
            }
            const wishlist = await fetchWishlist();
            const isInWishlist = wishlist.some(item => item.bookId === bookId);
            updateButtonIcon(button, isInWishlist);
        } catch (err) {
            console.error('Wishlist check failed:', err);
            updateButtonIcon(button, false);
        }
    }

    async function toggleWishlist() {
        if (!window.isAuthenticated) {
            window.location.href = '/User/Login';
            return;
        }

        try {
            const wishlist = await fetchWishlist();
            const isInWishlist = wishlist.some(item => item.bookId === bookId);

            const response = await fetch(
                isInWishlist
                    ? `https://localhost:7018/api/WishlistItem/delete/${bookId}`
                    : `https://localhost:7018/api/WishlistItem/add`,
                {
                    method: isInWishlist ? 'DELETE' : 'POST',
                    headers: {
                        'Authorization': `Bearer ${window.jwtToken}`,
                        ...(isInWishlist ? {} : { 'Content-Type': 'application/json' })
                    },
                    ...(isInWishlist ? {} : { body: JSON.stringify({ bookId }) })
                }
            );

            if (!response.ok) throw new Error('Wishlist update failed.');
            alert(isInWishlist ? 'Removed from wishlist!' : 'Added to wishlist!');
            await refreshIcon();
        } catch (error) {
            console.error('Wishlist update failed:', error);
            alert('Something went wrong. Try again.');
        }
    }

    await refreshIcon();
    button.addEventListener('click', e => {
        e.preventDefault();
        toggleWishlist();
    });
}

window.initializeWishlistButton = initializeWishlistButton;
