// Theme Management
class ThemeManager {
    constructor() {
        this.theme = localStorage.getItem('theme') || 'dark';
        this.init();
    }

    init() {
        this.applyTheme();
        this.bindEvents();
    }

    applyTheme() {
        document.documentElement.setAttribute('data-theme', this.theme);
        const themeIcon = document.querySelector('.theme-icon');
        if (themeIcon) {
            themeIcon.textContent = this.theme === 'dark' ? '☀️' : '🌙';
        }
    }

    toggle() {
        this.theme = this.theme === 'dark' ? 'light' : 'dark';
        localStorage.setItem('theme', this.theme);
        this.applyTheme();
    }

    bindEvents() {
        const themeToggle = document.getElementById('theme-toggle');
        if (themeToggle) {
            themeToggle.addEventListener('click', () => this.toggle());
        }
    }
}

// Tab System
class TabManager {
    constructor() {
        this.tabs = document.querySelectorAll('.tab-button');
        this.contents = document.querySelectorAll('.settings-demo');
        this.init();
    }

    init() {
        this.bindEvents();
    }

    bindEvents() {
        this.tabs.forEach(tab => {
            tab.addEventListener('click', (e) => this.switchTab(e.target.dataset.tab));
        });
    }

    switchTab(targetTab) {
        // Update tab buttons
        this.tabs.forEach(tab => {
            tab.classList.toggle('active', tab.dataset.tab === targetTab);
        });

        // Update content
        this.contents.forEach(content => {
            const shouldShow = content.id === `${targetTab}-demo`;
            content.classList.toggle('hidden', !shouldShow);
        });
    }
}

// Smooth Scrolling
class SmoothScroll {
    constructor() {
        this.init();
    }

    init() {
        this.bindEvents();
    }

    bindEvents() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', (e) => {
                e.preventDefault();
                const target = document.querySelector(anchor.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }
}

// Mobile Menu
class MobileMenu {
    constructor() {
        this.isOpen = false;
        this.init();
    }

    init() {
        this.createMobileMenu();
        this.bindEvents();
    }

    createMobileMenu() {
        // Create mobile menu overlay
        const overlay = document.createElement('div');
        overlay.className = 'mobile-menu-overlay hidden';
        overlay.innerHTML = `
            <div class="mobile-menu-content">
                <button class="mobile-menu-close">&times;</button>
                <nav class="mobile-nav">
                    <a href="#features" class="mobile-nav-link">Features</a>
                    <a href="#installation" class="mobile-nav-link">Installation</a>
                    <a href="#settings" class="mobile-nav-link">Settings</a>
                    <a href="#support" class="mobile-nav-link">Support</a>
                </nav>
            </div>
        `;
        document.body.appendChild(overlay);
        this.overlay = overlay;
    }

    bindEvents() {
        const toggle = document.querySelector('.mobile-menu-toggle');
        const closeBtn = document.querySelector('.mobile-menu-close');

        if (toggle) {
            toggle.addEventListener('click', () => this.toggle());
        }

        if (closeBtn) {
            closeBtn.addEventListener('click', () => this.close());
        }

        // Close on overlay click
        this.overlay.addEventListener('click', (e) => {
            if (e.target === this.overlay) {
                this.close();
            }
        });

        // Close on navigation link click
        this.overlay.querySelectorAll('.mobile-nav-link').forEach(link => {
            link.addEventListener('click', () => this.close());
        });
    }

    toggle() {
        this.isOpen = !this.isOpen;
        this.overlay.classList.toggle('hidden', !this.isOpen);
        document.body.style.overflow = this.isOpen ? 'hidden' : '';
    }

    close() {
        this.isOpen = false;
        this.overlay.classList.add('hidden');
        document.body.style.overflow = '';
    }
}

// Animation Observer
class AnimationObserver {
    constructor() {
        this.init();
    }

    init() {
        this.createObserver();
    }

    createObserver() {
        const options = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        this.observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-in');
                }
            });
        }, options);

        // Observe elements
        document.querySelectorAll('.feature-card, .step, .support-card').forEach(el => {
            this.observer.observe(el);
        });
    }
}

// Stats Counter Animation
class StatsCounter {
    constructor() {
        this.counters = document.querySelectorAll('.stat-number');
        this.init();
    }

    init() {
        this.createObserver();
    }

    createObserver() {
        const options = {
            threshold: 0.5
        };

        this.observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    this.animateCounter(entry.target);
                }
            });
        }, options);

        this.counters.forEach(counter => {
            this.observer.observe(counter);
        });
    }

    animateCounter(element) {
        const text = element.textContent;
        const hasPercent = text.includes('%');
        const hasVersion = text.includes('v');
        const number = parseFloat(text.replace(/[^0-9.]/g, ''));

        if (hasPercent) {
            this.countUp(element, 0, number, '%', 1000);
        } else if (hasVersion) {
            element.textContent = text; // Version numbers don't animate
        } else {
            this.countUp(element, 0, number, '', 1500);
        }
    }

    countUp(element, start, end, suffix, duration) {
        let startTime = null;

        const animate = (currentTime) => {
            if (startTime === null) startTime = currentTime;
            const progress = Math.min((currentTime - startTime) / duration, 1);
            const current = Math.floor(start + (end - start) * this.easeOut(progress));

            element.textContent = current + suffix;

            if (progress < 1) {
                requestAnimationFrame(animate);
            }
        };

        requestAnimationFrame(animate);
    }

    easeOut(t) {
        return 1 - Math.pow(1 - t, 3);
    }
}

// Copy Code Functionality
class CodeBlock {
    constructor() {
        this.init();
    }

    init() {
        this.createCopyButtons();
        this.bindEvents();
    }

    createCopyButtons() {
        document.querySelectorAll('.code-block').forEach(block => {
            const copyBtn = document.createElement('button');
            copyBtn.className = 'copy-btn';
            copyBtn.innerHTML = '📋';
            copyBtn.title = 'Copy to clipboard';

            block.style.position = 'relative';
            block.appendChild(copyBtn);
        });
    }

    bindEvents() {
        document.querySelectorAll('.copy-btn').forEach(btn => {
            btn.addEventListener('click', async (e) => {
                const codeBlock = e.target.parentElement;
                const codeText = codeBlock.textContent.replace('📋', '').trim();

                try {
                    await navigator.clipboard.writeText(codeText);
                    this.showFeedback(e.target, '✅ Copied!');
                } catch (err) {
                    this.showFeedback(e.target, '❌ Failed');
                }
            });
        });
    }

    showFeedback(button, message) {
        const original = button.innerHTML;
        button.innerHTML = message;
        button.style.background = message.includes('✅') ? '#28a745' : '#dc3545';

        setTimeout(() => {
            button.innerHTML = original;
            button.style.background = '';
        }, 2000);
    }
}

// Performance Monitoring
class PerformanceMonitor {
    constructor() {
        this.init();
    }

    init() {
        this.logLoadTime();
        this.trackInteractions();
    }

    logLoadTime() {
        window.addEventListener('load', () => {
            const loadTime = performance.now();
            console.log(`Page loaded in ${Math.round(loadTime)}ms`);
        });
    }

    trackInteractions() {
        // Track button clicks for analytics
        document.querySelectorAll('.btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const buttonText = e.target.textContent.trim();
                console.log(`Button clicked: ${buttonText}`);
            });
        });
    }
}

// Accessibility Enhancements
class AccessibilityManager {
    constructor() {
        this.init();
    }

    init() {
        this.enhanceKeyboardNavigation();
        this.addAriaLabels();
        this.manageFocus();
    }

    enhanceKeyboardNavigation() {
        // Tab navigation for settings tabs
        document.querySelectorAll('.tab-button').forEach((tab, index) => {
            tab.addEventListener('keydown', (e) => {
                if (e.key === 'ArrowRight' || e.key === 'ArrowLeft') {
                    e.preventDefault();
                    const direction = e.key === 'ArrowRight' ? 1 : -1;
                    const nextIndex = (index + direction + 3) % 3;
                    document.querySelectorAll('.tab-button')[nextIndex].focus();
                }
            });
        });
    }

    addAriaLabels() {
        // Add missing aria labels
        document.querySelectorAll('.mobile-menu-toggle').forEach(toggle => {
            toggle.setAttribute('aria-label', 'Toggle mobile menu');
        });
    }

    manageFocus() {
        // Trap focus in mobile menu when open
        const mobileMenu = document.querySelector('.mobile-menu-overlay');
        if (mobileMenu) {
            mobileMenu.addEventListener('keydown', (e) => {
                if (e.key === 'Escape') {
                    document.querySelector('.mobile-menu-close').click();
                }
            });
        }
    }
}

// Error Handling
class ErrorHandler {
    constructor() {
        this.init();
    }

    init() {
        this.setupGlobalErrorHandling();
    }

    setupGlobalErrorHandling() {
        window.addEventListener('error', (e) => {
            console.error('Global error:', e.error);
        });

        window.addEventListener('unhandledrejection', (e) => {
            console.error('Unhandled promise rejection:', e.reason);
        });
    }
}

// Initialize everything when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Initialize all managers
    const themeManager = new ThemeManager();
    const tabManager = new TabManager();
    const smoothScroll = new SmoothScroll();
    const mobileMenu = new MobileMenu();
    const animationObserver = new AnimationObserver();
    const statsCounter = new StatsCounter();
    const codeBlock = new CodeBlock();
    const performanceMonitor = new PerformanceMonitor();
    const accessibilityManager = new AccessibilityManager();
    const errorHandler = new ErrorHandler();

    // Add loading state management
    window.addEventListener('load', () => {
        document.body.classList.add('loaded');
    });

    console.log('New World AFK Preventer website initialized successfully!');
});
