const announcement = {
    async fetchAnnouncements() {
        try {
            const response = await fetch('https://localhost:7018/api/Announcement/getall', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Failed to fetch announcements.');
            }

            return await response.json();
        } catch (error) {
            Toast.fire({
                icon: 'error',
                title: error.message || 'Failed to fetch announcements'
            });
            return [];
        }
    },

    filterAnnouncements(announcements, user) {
        if (!announcements) return [];

        const userRoles = user ? ['All Users'] : ['All Users'];
        if (user?.isStaff) userRoles.push('Staff');
        if (user?.isAdmin) userRoles.push('Admin');

        const currentDate = new Date();
        currentDate.setHours(0, 0, 0, 0);

        return announcements.filter(announcement => {
            const isRoleMatch = announcement.recipientGroups.some(group => userRoles.includes(group));

            const startDate = new Date(announcement.startDate);
            startDate.setHours(0, 0, 0, 0);
            const endDate = new Date(announcement.endDate);
            endDate.setHours(0, 0, 0, 0);
            const isWithinDateRange = currentDate >= startDate && currentDate <= endDate;

            return isRoleMatch && isWithinDateRange;
        });
    },

    renderAnnouncements(announcements) {
        const listElement = document.getElementById('announcements-list');
        const noAnnouncements = document.getElementById('no-announcements');

        listElement.innerHTML = '';

        if (announcements.length === 0) {
            noAnnouncements.classList.remove('hidden');
            return;
        }

        noAnnouncements.classList.add('hidden');

        announcements.forEach(announcement => {
            const announcementItem = document.createElement('div');
            announcementItem.className = 'bg-gray-100 p-4 rounded border border-accent1';
            announcementItem.innerHTML = `
                <h3 class="text-accent3 font-semibold text-lg">${announcement.title}</h3>
                <p class="text-accent2">${announcement.message || 'No message'}</p>
                <p class="text-accent1 text-sm">From: ${new Date(announcement.startDate).toLocaleDateString()} | To: ${new Date(announcement.endDate).toLocaleDateString()}</p>
            `;
            listElement.appendChild(announcementItem);
        });
    },

    init(user) {
        const icon = document.getElementById('announcements-icon');
        const modal = document.getElementById('announcements-modal');
        const closeModal = document.getElementById('close-announcements-modal');

        if (!icon || !modal || !closeModal) {
            console.error('Required DOM elements for announcements are missing.');
            return;
        }

        icon.addEventListener('click', async () => {
            const announcements = await this.fetchAnnouncements();
            if (!announcements || announcements.length === 0) {
                document.getElementById('no-announcements').classList.remove('hidden');
                document.getElementById('announcements-list').innerHTML = '';
                modal.classList.remove('hidden');
                return;
            }

            // Pass user object, which can be null for non-authenticated users
            const filteredAnnouncements = this.filterAnnouncements(announcements, user);
            this.renderAnnouncements(filteredAnnouncements);
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