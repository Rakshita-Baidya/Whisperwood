const createPaginationControls = (containerId, totalPages, currentPage, onPageChange) => {
    const container = document.getElementById(containerId);
    if (!container) return;

    container.innerHTML = '';

    const createSvgButton = (dir) => {
        const btn = document.createElement('button');
        btn.className = `p-2 rounded-full mx-1 flex items-center justify-center ${(dir === 'prev' && currentPage === 1) || (dir === 'next' && currentPage === totalPages)
                ? 'bg-gray-300 cursor-not-allowed'
                : 'bg-accent3 hover:bg-accent2'
            }`;
        btn.disabled = (dir === 'prev' && currentPage === 1) || (dir === 'next' && currentPage === totalPages);
        btn.onclick = () => {
            if (dir === 'prev' && currentPage > 1) onPageChange(currentPage - 1);
            if (dir === 'next' && currentPage < totalPages) onPageChange(currentPage + 1);
        };
        btn.innerHTML = dir === 'prev'
            ? `<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2"><path d="M15 18l-6-6 6-6"/></svg>`
            : `<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2"><path d="M9 18l6-6-6-6"/></svg>`;
        return btn;
    };

    // Prev button
    container.appendChild(createSvgButton('prev'));

    // Page numbers
    const maxPagesToShow = 5;
    let startPage = Math.max(1, currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(totalPages, startPage + maxPagesToShow - 1);
    startPage = Math.max(1, endPage - maxPagesToShow + 1);

    for (let i = startPage; i <= endPage; i++) {
        const pageButton = document.createElement('button');
        pageButton.textContent = i;
        pageButton.className = `px-3 py-1 rounded mx-1 text-sm font-bold ${i === currentPage
                ? 'bg-accent3 text-white'
                : 'bg-primary text-accent3 border-accent1 border-[2px] hover:bg-accent2'
            }`;
        pageButton.onclick = () => onPageChange(i);
        container.appendChild(pageButton);
    }

    // Next button
    container.appendChild(createSvgButton('next'));
};

window.createPaginationControls = createPaginationControls;
