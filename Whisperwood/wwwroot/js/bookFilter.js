function getFilterParams() {
    const form = document.getElementById('filter-form');
    if (!form) {
        console.error('Filter form not found');
        return {};
    }

    const formData = new FormData(form);
    return {
        authorIds: formData.getAll('authorIds'),
        genreIds: formData.getAll('genreIds'),
        publisherIds: formData.getAll('publisherIds'),
        formats: formData.getAll('formats').map(Number),
        searchTerm: formData.get('search') || null,
        minPrice: formData.get('minPrice') ? parseFloat(formData.get('minPrice')) : null,
        maxPrice: formData.get('maxPrice') ? parseFloat(formData.get('maxPrice')) : null,
        minRating: formData.get('minRating') ? parseFloat(formData.get('minRating')) : null,
        language: formData.get('language') || null,
        isAvailable: formData.get('isAvailable') ? formData.get('isAvailable') === 'true' : null,
        sortBy: formData.get('sortBy') ? parseInt(formData.get('sortBy')) : null,
        sortOrder: formData.get('sortOrder') ? parseInt(formData.get('sortOrder')) : null
    };
}

function filterBooks(books, params) {
    let filteredBooks = [...books];

    // Apply filters
    if (params.authorIds.length > 0) {
        filteredBooks = filteredBooks.filter(book =>
            book.authorBooks.some(ab => params.authorIds.includes(ab.author.id.toString()))
        );
    }

    if (params.genreIds.length > 0) {
        filteredBooks = filteredBooks.filter(book =>
            book.genreBooks.some(gb => params.genreIds.includes(gb.genre.id.toString()))
        );
    }

    if (params.publisherIds.length > 0) {
        filteredBooks = filteredBooks.filter(book =>
            params.publisherIds.includes(book.publisherId.toString())
        );
    }

    if (params.formats.length > 0) {
        filteredBooks = filteredBooks.filter(book =>
            params.formats.includes(book.format)
        );
    }

    if (params.searchTerm) {
        const searchLower = params.searchTerm.toLowerCase();
        filteredBooks = filteredBooks.filter(book =>
            book.title.toLowerCase().includes(searchLower) ||
            book.isbn.includes(searchLower) ||
            book.synopsis.toLowerCase().includes(searchLower)   
        );
    }

    if (params.minPrice !== null) {
        filteredBooks = filteredBooks.filter(book => book.price >= params.minPrice);
    }

    if (params.maxPrice !== null) {
        filteredBooks = filteredBooks.filter(book => book.price <= params.maxPrice);
    }

    if (params.minRating !== null) {
        filteredBooks = filteredBooks.filter(book => book.averageRating >= params.minRating);
    }

    if (params.language) {
        filteredBooks = filteredBooks.filter(book =>
            book.language && book.language.toLowerCase() === params.language.toLowerCase()
        );
    }

    if (params.isAvailable !== null) {
        filteredBooks = filteredBooks.filter(book => book.stock > 0 === params.isAvailable);
    }

    // Apply sorting
    if (params.sortBy !== null) {
        filteredBooks.sort((a, b) => {
            let valueA, valueB;
            switch (params.sortBy) {
                case 0: // Title
                    valueA = a.title.toLowerCase();
                    valueB = b.title.toLowerCase();
                    break;
                case 1: // Publication Date
                    valueA = new Date(a.publicationDate);
                    valueB = new Date(b.publicationDate);
                    break;
                case 2: // Price
                    valueA = a.price;
                    valueB = b.price;
                    break;
                case 3: // Popularity
                    valueA = a.averageRating || 0;
                    valueB = b.averageRating || 0;
                    break;
            }
            if (params.sortOrder === 0) { // Ascending
                return valueA < valueB ? -1 : valueA > valueB ? 1 : 0;
            } else { // Descending
                return valueA > valueB ? -1 : valueA < valueB ? 1 : 0;
            }
        });
    }

    return filteredBooks;
}

async function fetchDropdownOptions() {
    try {
        // Fetch authors
        const authorsResponse = await fetch('https://localhost:7018/api/Author/getall');
        if (!authorsResponse.ok) throw new Error('Failed to fetch authors');
        const authors = await authorsResponse.json();
        const authorsSelect = document.getElementById('authors');
        if (authorsSelect) {
            authors.forEach(author => {
                const option = document.createElement('option');
                option.value = author.id;
                option.textContent = author.name;
                authorsSelect.appendChild(option);
            });
        }

        // Fetch genres
        const genresResponse = await fetch('https://localhost:7018/api/Genre/getall');
        if (!genresResponse.ok) throw new Error('Failed to fetch genres');
        const genres = await genresResponse.json();
        const genresSelect = document.getElementById('genres');
        if (genresSelect) {
            genres.forEach(genre => {
                const option = document.createElement('option');
                option.value = genre.id;
                option.textContent = genre.name;
                genresSelect.appendChild(option);
            });
        }

        // Fetch publishers
        const publishersResponse = await fetch('https://localhost:7018/api/Publisher/getall');
        if (!publishersResponse.ok) throw new Error('Failed to fetch publishers');
        const publishers = await publishersResponse.json();
        const publishersSelect = document.getElementById('publishers');
        if (publishersSelect) {
            publishers.forEach(publisher => {
                const option = document.createElement('option');
                option.value = publisher.id;
                option.textContent = publisher.name;
                publishersSelect.appendChild(option);
            });
        }
    } catch (error) {
        console.error('Error fetching dropdown options:', error);
        const errorMessageElement = document.getElementById('error-message');
        if (errorMessageElement) {
            errorMessageElement.textContent = 'Failed to load filter options.';
            errorMessageElement.classList.remove('hidden');
        }
    }
}

function clearFilters() {
    const form = document.getElementById('filter-form');
    if (!form) {
        console.error('Filter form not found');
        return;
    }
    form.reset();
    const selects = form.querySelectorAll('select[multiple]');
    selects.forEach(select => {
        Array.from(select.options).forEach(option => option.selected = false);
    });
}