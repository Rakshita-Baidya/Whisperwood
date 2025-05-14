const createPaginationControls = (containerId, totalPages, currentPage, onPageChange) => {
    const container = document.getElementById(containerId);
    if (!container) return;

    container.innerHTML = '';

    // Previous button
    const prevButton = document.createElement('button');
    prevButton.textContent = '<';
    prevButton.className = `px-3 py-1 rounded ${currentPage === 1 ? 'bg-gray-300 cursor-not-allowed' : 'bg-accent3 text-white hover:bg-accent2'}`;
    prevButton.disabled = currentPage === 1;
    prevButton.onclick = () => {
        if (currentPage > 1) onPageChange(currentPage - 1);
    };
    container.appendChild(prevButton);

    // Page numbers
    const maxPagesToShow = 5;
    let startPage = Math.max(1, currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(totalPages, startPage + maxPagesToShow - 1);
    startPage = Math.max(1, endPage - maxPagesToShow + 1);

    for (let i = startPage; i <= endPage; i++) {
        const pageButton = document.createElement('button');
        pageButton.textContent = i;
        pageButton.className = `px-3 py-1 rounded mx-1 ${i === currentPage ? 'bg-accent3 text-white' : 'bg-primary border-accent1 border-[2px] hover:bg-accent2'}`;
        pageButton.onclick = () => onPageChange(i);
        container.appendChild(pageButton);
    }

    // Next button
    const nextButton = document.createElement('button');
    nextButton.textContent = '>';
    nextButton.className = `px-3 py-1 rounded ${currentPage === totalPages ? 'bg-gray-300 cursor-not-allowed' : 'bg-accent3 text-white hover:bg-accent2'}`;
    nextButton.disabled = currentPage === totalPages;
    nextButton.onclick = () => {
        if (currentPage < totalPages) onPageChange(currentPage + 1);
    };
    container.appendChild(nextButton);
};

window.createPaginationControls = createPaginationControls;