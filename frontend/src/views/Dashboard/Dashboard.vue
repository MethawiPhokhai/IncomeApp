<template>
  <div class="dashboard">
    <!-- Loading State -->
    <div v-if="loading" class="loading-container">
      <div class="spinner"></div>
      <p>กำลังโหลดข้อมูล...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="error-container">
      <p class="error-message">{{ (error as Error).message }}</p>
      <button @click="() => refetch()" class="btn-retry">ลองอีกครั้ง</button>
    </div>

    <!-- Dashboard Content -->
    <div v-else-if="dashboard" class="dashboard-content">
      <!-- Section 1: The Big Numbers - Money Flow -->
      <section class="section-summary">
        <h2 class="section-title">Portfolio Overview</h2>
        <div class="hero-summary-card">
          <div class="hero-top">
            <div class="hero-label">NET WORTH GROWTH</div>
            <div class="hero-top-right">
              <div class="hero-badge" :class="dashboard.netWorthGrowth >= 0 ? 'badge-positive' : 'badge-negative'">
                {{ dashboard.netWorthGrowth >= 0 ? '+' : '' }}{{ formatPercent(dashboard.netWorthGrowthPercent) }}
              </div>
              <button class="btn-edit-summary" @click="showSummaryModal = true" title="Edit summary">
                <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
                  <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
                </svg>
              </button>
            </div>
          </div>
          <div class="hero-value">{{ formatCurrency(dashboard.netWorthGrowth) }}</div>

          <div class="hero-divider"></div>

          <div class="hero-metrics-row">
            <div class="hero-metric">
              <span class="hero-metric-label">MONTHLY INCOME</span>
              <span class="hero-metric-value">{{ formatCurrency(dashboard.income) }}</span>
            </div>
            <div class="hero-metric">
              <span class="hero-metric-label">MONTHLY EXPENSES</span>
              <span class="hero-metric-value">{{ formatCurrency(dashboard.totalExpenses) }}</span>
            </div>
            <div class="hero-metric">
              <span class="hero-metric-label">SAVINGS + INVESTMENT</span>
              <span class="hero-metric-value">{{ formatCurrency(dashboard.totalSavings + dashboard.totalInvestment) }}</span>
            </div>
            <div class="hero-metric">
              <span class="hero-metric-label">SAVINGS RATE</span>
              <span class="hero-metric-value hero-metric-value--accent">{{ (((dashboard.totalSavings + dashboard.totalInvestment) / dashboard.income) * 100).toFixed(1) }}%</span>
            </div>
          </div>
        </div>
      </section>

      <!-- Section 2: App Summary with Subscriptions -->
      <section class="section-categories">
        <h2 class="section-title">Expense Categories</h2>

        <div class="app-summary-full">
          <AppExpenseSummary
            :categories="dashboard.categories"
            :subscriptions="dashboard.subscriptions"
            @refresh="loadDashboard"
          />
        </div>
      </section>

      <!-- Modals -->
      <SummaryEditModal
        :is-open="showSummaryModal"
        :current-data="dashboard"
        @close="showSummaryModal = false"
        @save="handleSaveSummary"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { type DashboardSummary, type UpdateSummaryRequest } from '../../services/financialService'
import { formatCurrency, formatPercent } from '../../utils/formatters'
import { useApi, useApiMutation } from '../../composables/useCrud'

// Components
import AppExpenseSummary from '../../components/AppExpenseSummary/AppExpenseSummary.vue'
import SummaryEditModal from '../../components/SummaryEditModal/SummaryEditModal.vue'

// ============================================================================
// Composables
// ============================================================================
const router = useRouter()

// ============================================================================
// Query & Mutation
// ============================================================================
const { data: dashboard, isLoading: loading, error, refetch } = useApi<DashboardSummary>(
  '/api/financial/dashboard'
)

const updateSummaryMutation = useApiMutation<DashboardSummary, UpdateSummaryRequest>(
  'post',
  '/api/financial/summary',
  { invalidateKeys: [['dashboard']] }
)

// ============================================================================
// Reactive State
// ============================================================================
const showSummaryModal = ref(false)

// ============================================================================
// Component Functions
// ============================================================================
const loadDashboard = () => refetch()

const handleSaveSummary = async (data: UpdateSummaryRequest) => {
  await updateSummaryMutation.mutateAsync(data)
  showSummaryModal.value = false
}

// ============================================================================
// Lifecycle Hooks
// ============================================================================
onMounted(() => {
  const token = localStorage.getItem('token')
  if (!token) {
    router.push('/')
  }
})
</script>

<style scoped src="./Dashboard.css"></style>
