import { apiClient } from './apiClient';

export interface DashboardSummary {
    income: number;
    totalSavings: number;
    totalInvestment: number;
    totalExpenses: number;
    netWorthGrowth: number;
    netWorthGrowthPercent: number;
    savingRate: number;
    burnRate: number;
    dailyBudget: number;
    categories: CategoryBreakdown[];
    subscriptions: Subscription[];
    insurances: Insurance[];
    debts: Debt[];
    charts: ChartData;
}

export interface CategoryBreakdown {
    id?: string;
    name: string;
    amount: number;
    type: string;
    color: string;
    bankApp: string;
    isHighlighted?: boolean;
}

export interface Subscription {
    name: string;
    amount: number;
    billingCycle: string;
    nextBillingDate: string;
    remark: string;
    bankApp: string;
}

export interface Insurance {
    id?: string;
    provider: string;
    policyName: string;
    premium: number;
    dueDate: string;
    status: string;
}

export interface Debt {
    id?: string;
    name: string;
    monthlyPayment: number;
    currentInstallment: number;
    totalInstallments: number;
    remainingAmount: number;
    totalAmount: number;
}

export interface ChartData {
    expensesByApp: ChartDataPoint[];
    topExpenses: ChartDataPoint[];
}

export interface ChartDataPoint {
    label: string;
    value: number;
    color: string;
}

export interface UpdateSummaryRequest {
    income: number;
    totalSavings: number;
    totalInvestment: number;
    netWorthGrowth: number;
}

export const financialService = {
    async getDashboard(signal?: AbortSignal): Promise<DashboardSummary> {
        return apiClient.get<DashboardSummary>('/api/financial/dashboard', signal);
    },
    async updateSummary(data: UpdateSummaryRequest, signal?: AbortSignal): Promise<DashboardSummary> {
        return apiClient.post<DashboardSummary>('/api/financial/summary', data, signal);
    },

    // Expense CRUD
    async addExpense(expense: CategoryBreakdown, signal?: AbortSignal): Promise<CategoryBreakdown> {
        return apiClient.post<CategoryBreakdown>('/api/financial/expenses', expense, signal);
    },
    async updateExpense(id: string, expense: CategoryBreakdown, signal?: AbortSignal): Promise<CategoryBreakdown> {
        return apiClient.put<CategoryBreakdown>(`/api/financial/expenses/${id}`, expense, signal);
    },
    async deleteExpense(id: string, signal?: AbortSignal): Promise<void> {
        return apiClient.delete(`/api/financial/expenses/${id}`, signal);
    },

    // Insurance CRUD
    async addInsurance(insurance: Insurance, signal?: AbortSignal): Promise<Insurance> {
        return apiClient.post<Insurance>('/api/financial/insurances', insurance, signal);
    },
    async updateInsurance(id: string, insurance: Insurance, signal?: AbortSignal): Promise<Insurance> {
        return apiClient.put<Insurance>(`/api/financial/insurances/${id}`, insurance, signal);
    },
    async deleteInsurance(id: string, signal?: AbortSignal): Promise<void> {
        return apiClient.delete(`/api/financial/insurances/${id}`, signal);
    },

    // Debt CRUD
    async addDebt(debt: Debt, signal?: AbortSignal): Promise<Debt> {
        return apiClient.post<Debt>('/api/financial/debts', debt, signal);
    },
    async updateDebt(id: string, debt: Debt, signal?: AbortSignal): Promise<Debt> {
        return apiClient.put<Debt>(`/api/financial/debts/${id}`, debt, signal);
    },
    async deleteDebt(id: string, signal?: AbortSignal): Promise<void> {
        return apiClient.delete(`/api/financial/debts/${id}`, signal);
    }
};

