const promotion = {
    async fetchPromotions() {
        try {
            const response = await fetch('https://localhost:7018/api/Promotion/getall', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${window.jwtToken}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Failed to fetch promotions.');
            }

            return await response.json();
        } catch (error) {
            console.error('Error fetching promotions:', error);
            return null;
        }
    },

    filterPromotions(promotions, user) {
        if (!user || !promotions) return [];

        const currentDate = new Date();
        currentDate.setHours(0, 0, 0, 0);

        return promotions.filter(promotion => {
            const startDate = new Date(promotion.startDate);
            startDate.setHours(0, 0, 0, 0);
            const endDate = new Date(promotion.endDate);
            endDate.setHours(0, 0, 0, 0);
            const isWithinDateRange = currentDate >= startDate && currentDate <= endDate;

            return isWithinDateRange;
        });
    },

    renderPromotions(promotions) {
        const listElement = document.getElementById('promotions-list');
        const noPromotions = document.getElementById('no-promotions');

        if (!listElement || !noPromotions) {
            console.error('Promotion modal elements not found:', {
                listElement: !!listElement,
                noPromotions: !!noPromotions
            });
            return;
        }

        listElement.innerHTML = '';

        if (promotions.length === 0) {
            noPromotions.classList.remove('hidden');
            return;
        }

        noPromotions.classList.add('hidden');

        promotions.forEach(promotion => {
            const promotionItem = document.createElement('div');
            promotionItem.className = 'bg-gray-100 p-4 rounded-lg border border-accent1';
            promotionItem.innerHTML = `
                <h3 class="text-accent3 font-semibold text-lg">${promotion.name}</h3>
                <p class="text-accent2">${promotion.description || 'No message'}</p>
                <p class="text-accent2"> Get ${promotion.discountPercent}% off by using the code '${promotion.code}' in your next order!</p>
                <p class="text-accent1 text-sm">From: ${promotion.startDate} | To: ${promotion.endDate}</p>
            `;
            listElement.appendChild(promotionItem);
        });
    },

    init(user) {
        const icon = document.getElementById('promotions-icon');
        const modal = document.getElementById('promotions-modal');
        const closeModal = document.getElementById('close-promotions-modal');
        const errorElement = document.getElementById('promotions-error');

        if (!icon || !modal || !closeModal || !errorElement) {
            console.error('Promotion DOM elements not found:', {
                icon: !!icon,
                modal: !!modal,
                closeModal: !!closeModal,
                errorElement: !!errorElement
            });
            return;
        }

        icon.addEventListener('click', async () => {
            errorElement.classList.add('hidden');

            const promotions = await this.fetchPromotions();
            if (!promotions) {
                errorElement.textContent = 'Failed to load promotions.';
                errorElement.classList.remove('hidden');
                return;
            }

            const filteredPromotions = this.filterPromotions(promotions, user);
            this.renderPromotions(filteredPromotions);

            modal.classList.remove('hidden');
        });

        closeModal.addEventListener('click', () => {
            modal.classList.add('hidden');
        });

        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                modal.classList.add('hidden');
            }
        });
    }
};