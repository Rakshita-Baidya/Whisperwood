// gets book filter parameters from form
function getBookFilterParams() {
    const form = document.getElementById('filter-form');
    if (!form) {
        console.warn('filter form not found');
        return {};
    }

    const formData = new FormData(form);
    return {
        authorIds: formData.getAll('authorIds').map(String),
        genreIds: formData.getAll('genreIds').map(String),
        publisherIds: formData.getAll('publisherIds').map(String),
        formats: formData.getAll('formats').map(Number),
        searchTerm: formData.get('search')?.trim().toLowerCase() || null,
        minPrice: formData.get('minPrice') ? parseFloat(formData.get('minPrice')) : null,
        maxPrice: formData.get('maxPrice') ? parseFloat(formData.get('maxPrice')) : null,
        minRating: formData.get('minRating') ? parseFloat(formData.get('minRating')) : null,
        language: formData.get('language')?.trim().toLowerCase() || null,
        isAvailable: formData.get('isAvailable') ? formData.get('isAvailable') === 'true' : null,
        sortBy: formData.get('sortBy') ? parseInt(formData.get('sortBy'), 10) : null,
        sortOrder: formData.get('sortOrder') ? parseInt(formData.get('sortOrder'), 10) : null
    };
}

// validates book filter parameters
function validateBookFilterParams(params) {
    if (params.minPrice !== null && params.minPrice < 0) {
        Toast.fire({ icon: 'error', title: 'Min price cannot be negative' });
        return false;
    }
    if (params.maxPrice !== null && params.maxPrice < 0) {
        Toast.fire({ icon: 'error', title: 'Max price cannot be negative' });
        return false;
    }
    if (params.minPrice !== null && params.maxPrice !== null && params.maxPrice < params.minPrice) {
        Toast.fire({ icon: 'error', title: 'Max price must be greater than or equal to min price' });
        return false;
    }
    if (params.minRating !== null && (params.minRating < 0 || params.minRating > 5)) {
        Toast.fire({ icon: 'error', title: 'Min rating must be between 0 and 5' });
        return false;
    }
    if (params.language && !/^[a-zA-Z\s]+$/.test(params.language)) {
        Toast.fire({ icon: 'error', title: 'Language must contain only letters' });
        return false;
    }
    if (params.authorIds.length === 0 && document.getElementById('authors')?.selectedOptions.length > 0) {
        Toast.fire({ icon: 'error', title: 'At least one author must be selected' });
        return false;
    }
    if (params.genreIds.length === 0 && document.getElementById('genres')?.selectedOptions.length > 0) {
        Toast.fire({ icon: 'error', title: 'At least one genre must be selected' });
        return false;
    }
    if (params.publisherIds.length === 0 && document.getElementById('publishers')?.selectedOptions.length > 0) {
        Toast.fire({ icon: 'error', title: 'At least one publisher must be selected' });
        return false;
    }
    return true;
}

// filters books based on parameters
function filterBooks(books, params) {
    return books
        .filter(book => params.authorIds.length === 0 || book.authorBooks?.some(ab => params.authorIds.includes(ab.author.id.toString())))
        .filter(book => params.genreIds.length === 0 || book.genreBooks?.some(gb => params.genreIds.includes(gb.genre.id.toString())))
        .filter(book => params.publisherIds.length === 0 || book.publisherBooks?.some(pb => params.publisherIds.includes(pb.publisher.id.toString())))
        .filter(book => params.formats.length === 0 || params.formats.includes(book.format))
        .filter(book => {
            if (params.searchTerm) {
                const searchLower = params.searchTerm.toLowerCase();
                return book.title.toLowerCase().includes(searchLower) ||
                    book.isbn?.toLowerCase().includes(searchLower) ||
                    book.synopsis?.toLowerCase().includes(searchLower);
            }
            return true;
        })
        .filter(book => params.minPrice === null || window.calculateBookPrice(book) >= params.minPrice)
        .filter(book => params.maxPrice === null || window.calculateBookPrice(book) <= params.maxPrice)
        .filter(book => params.minRating === null || (book.averageRating || 0) >= params.minRating)
        .filter(book => params.language === null || book.language?.toLowerCase() === params.language)
        .filter(book => params.isAvailable === null || book.stock > 0 === params.isAvailable)
        .sort((a, b) => {
            if (params.sortBy === null) return 0;
            let valueA, valueB;
            switch (params.sortBy) {
                case 0: // title
                    valueA = a.title.toLowerCase();
                    valueB = b.title.toLowerCase();
                    break;
                case 1: // publication date
                    valueA = new Date(a.publicationDate);
                    valueB = new Date(b.publicationDate);
                    break;
                case 2: // price
                    valueA = window.calculateBookPrice(a);
                    valueB = window.calculateBookPrice(b);
                    break;
                case 3: // popularity
                    valueA = a.averageRating || 0;
                    valueB = b.averageRating || 0;
                    break;
            }
            const comparison = valueA < valueB ? -1 : valueA > valueB ? 1 : 0;
            return params.sortOrder === 0 ? comparison : -comparison;
        });
}

// fetches dropdown options for book filters
async function fetchBookDropdownOptions() {
    const authorsSelect = document.getElementById('authors');
    const genresSelect = document.getElementById('genres');
    const categoriesSelect = document.getElementById('categories');
    const publishersSelect = document.getElementById('publishers');

    try {
        // fetch authors
        const authorsResponse = await fetch('https://localhost:7018/api/Author/getall');
        if (!authorsResponse.ok) throw new Error('Failed to fetch authors');
        const authors = await authorsResponse.json();
        authorsSelect.innerHTML = authors.map(a => `<option value="${a.id}">${a.name}</option>`).join('');

        // fetch genres
        const genresResponse = await fetch('https://localhost:7018/api/Genre/getall');
        if (!genresResponse.ok) throw new Error('Failed to fetch genres');
        const genres = await genresResponse.json();
        genresSelect.innerHTML = genres.map(g => `<option value="${g.id}">${g.name}</option>`).join('');

        // fetch categories
        if (categoriesSelect) {
            const categoriesResponse = await fetch('https://localhost:7018/api/Category/getall');
            if (!categoriesResponse.ok) throw new Error('Failed to fetch categories');
            const categories = await categoriesResponse.json();
            categoriesSelect.innerHTML = categories.map(c => `<option value="${c.id}">${c.name}</option>`).join('');
        }

        // fetch publishers
        const publishersResponse = await fetch('https://localhost:7018/api/Publisher/getall');
        if (!publishersResponse.ok) throw new Error('Failed to fetch publishers');
        const publishers = await publishersResponse.json();
        publishersSelect.innerHTML = publishers.map(p => `<option value="${p.id}">${p.name}</option>`).join('');
    } catch (error) {
        Toast.fire({
            icon: 'error',
            title: error.message || 'Failed to load dropdown options'
        });
    }
}

// clears book filter form inputs
function clearBookFilters() {
    const form = document.getElementById('filter-form');
    if (!form) {
        console.warn('filter form not found');
        return;
    }
    form.reset();
}