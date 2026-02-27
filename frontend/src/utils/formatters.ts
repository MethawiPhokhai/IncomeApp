/**
 * Format number as Thai Baht currency
 */
export const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('th-TH', {
        style: 'currency',
        currency: 'THB',
        minimumFractionDigits: 0,
        maximumFractionDigits: 2
    }).format(amount);
};

/**
 * Format number with commas
 */
export const formatNumber = (num: number, decimals: number = 0): string => {
    return new Intl.NumberFormat('th-TH', {
        minimumFractionDigits: decimals,
        maximumFractionDigits: decimals
    }).format(num);
};

/**
 * Format as percentage
 */
export const formatPercent = (num: number, decimals: number = 2): string => {
    return `${formatNumber(num, decimals)}%`;
};

/**
 * Helper to normalize date and handle Buddhist Era (BE) years
 * Some devices/inputs might produce years like 2567 instead of 2024
 */
const normalizeDate = (date: string | Date): Date => {
    const d = typeof date === 'string' ? new Date(date) : new Date(date.getTime());
    if (d.getFullYear() > 2500) {
        d.setFullYear(d.getFullYear() - 543);
    }
    return d;
};

/**
 * Format date in Thai locale
 */
export const formatDate = (date: string | Date): string => {
    const d = normalizeDate(date);
    return new Intl.DateTimeFormat('th-TH', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    }).format(d);
};

/**
 * Format short date (day/month)
 */
export const formatShortDate = (date: string | Date): string => {
    const d = normalizeDate(date);
    return new Intl.DateTimeFormat('th-TH', {
        month: 'short',
        day: 'numeric'
    }).format(d);
};

/**
 * Get days until a date
 */
export const getDaysUntil = (date: string | Date): number => {
    const d = normalizeDate(date);
    const now = new Date();

    // Reset time part to accurately calculate full days
    d.setHours(0, 0, 0, 0);
    now.setHours(0, 0, 0, 0);

    const diff = d.getTime() - now.getTime();
    return Math.round(diff / (1000 * 60 * 60 * 24));
};
