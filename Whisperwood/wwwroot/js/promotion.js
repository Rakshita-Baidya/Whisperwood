const promotion = {
    async fetchPromotions() {
        try {
            const response = await fetch('https://localhost:7018/api/Promotion/getall', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Failed to fetch promotions');
            }

            return await response.json();
        } catch (error) {
            Toast.fire({
                icon: 'error',
                title: error.message || 'Failed to fetch promotions'
            });
            return [];
        }
    },

    filterPromotions(promotions, user) {
        if (!promotions) return [];

        const currentDate = new Date();
        currentDate.setHours(0, 0, 0, 0);

        return promotions.filter(promotion => {
            const startDate = new Date(promotion.startDate);
            startDate.setHours(0, 0, 0, 0);
            const endDate = new Date(promotion.endDate);
            endDate.setHours(0, 0, 0, 0);
            return currentDate >= startDate && currentDate <= endDate;
        });
    },

    renderPromotions(promotions, elements) {
        elements.list.innerHTML = '';

        if (promotions.length === 0) {
            elements.noPromotions.classList.remove('hidden');
            return;
        }

        elements.noPromotions.classList.add('hidden');

        promotions.forEach(promotion => {
            const promotionItem = document.createElement('div');
            promotionItem.className = 'bg-primary p-4 rounded border border-accent1';
            promotionItem.innerHTML = `
                <h3 class="text-accent3 font-semibold text-lg">${promotion.name}</h3>
                <p class="text-accent2">${promotion.description || 'No description'}</p>
                <p class="text-accent2">Get ${promotion.discountPercent}% off by using code '<span class="font-semibold">${promotion.code}</span>' in your next order!</p>
                <p class="text-accent1 text-sm">From: ${new Date(promotion.startDate).toLocaleDateString()} | To: ${new Date(promotion.endDate).toLocaleDateString()}</p>
                ${!window.isAuthenticated ? '<p class="text-red-600 text-sm mt-2">Please log in to use this promotion code.</p>' : ''}
            `;
            elements.list.appendChild(promotionItem);
        });
    },

    init(user) {
        const elements = {
            icon: document.getElementById('promotions-icon'),
            modal: document.getElementById('promotions-modal'),
            closeModal: document.getElementById('close-promotions-modal'),
            list: document.getElementById('promotions-list'),
            noPromotions: document.getElementById('no-promotions')
        };

        elements.icon.addEventListener('click', async () => {
            const promotions = await this.fetchPromotions();
            const filteredPromotions = this.filterPromotions(promotions, user);
            this.renderPromotions(filteredPromotions, elements);
            elements.modal.classList.remove('hidden');
        });

        elements.closeModal.addEventListener('click', () => {
            elements.modal.classList.add('hidden');
        });

        elements.modal.addEventListener('click', (e) => {
            if (e.target === elements.modal) {
                elements.modal.classList.add('hidden');
            }
        });
    }
};