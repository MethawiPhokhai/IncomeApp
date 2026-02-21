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
 * Format date in Thai locale
 */
export const formatDate = (date: string | Date): string => {
    const d = typeof date === 'string' ? new Date(date) : date;
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
    const d = typeof date === 'string' ? new Date(date) : date;
    return new Intl.DateTimeFormat('th-TH', {
        month: 'short',
        day: 'numeric'
    }).format(d);
};

/**
 * Get days until a date
 */
export const getDaysUntil = (date: string | Date): number => {
    const d = typeof date === 'string' ? new Date(date) : date;
    const now = new Date();
    const diff = d.getTime() - now.getTime();
    return Math.ceil(diff / (1000 * 60 * 60 * 24));
};
