<template>
  <div class="dashboard">
    <!-- Header -->
    <header class="dashboard-header">
      <div class="header-content">
        <h1 class="dashboard-title">Dashboard</h1>
        <div class="header-actions">
          <ThemeToggle />
          <button @click="goToAnalytics" class="btn-analytics" title="ภาพรวมทางการเงิน">📊</button>
          <button @click="handleLogout" class="btn-logout">Sign out</button>
        </div>
      </div>
    </header>

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
        <div class="summary-header" style="display: flex; justify-content: flex-end; margin-bottom: 0.5rem;">
           <button class="btn-edit-summary" @click="showSummaryModal = true" style="background: none; border: none; cursor: pointer; color: #6b7280; font-size: 1.25rem; display: flex; align-items: center; justify-content: center; width: 32px; height: 32px; border-radius: 50%; transition: background-color 0.2s;">
             ✏️
           </button>
        </div>
        <div class="summary-grid">
          <SummaryCard 
            label="รายได้"
            :value="formatCurrency(dashboard.income)"
            variant="info"
          >
            <template #icon>💰</template>
          </SummaryCard>

          <SummaryCard 
            label="ค่าใช้จ่าย"
            :value="formatCurrency(dashboard.totalExpenses)"
            :subtitle="`${((dashboard.totalExpenses / dashboard.income) * 100).toFixed(1)}% ของรายได้`"
            variant="warning"
          >
            <template #icon>💸</template>
          </SummaryCard>

          <SummaryCard 
            label="เงินออม + ลงทุน"
            :value="formatCurrency(dashboard.totalSavings + dashboard.totalInvestment)"
            :subtitle="`${(((dashboard.totalSavings + dashboard.totalInvestment) / dashboard.income) * 100).toFixed(1)}% ของรายได้`"
            variant="success"
          >
            <template #icon>🎯</template>
          </SummaryCard>

          <SummaryCard 
            label="Net Worth Growth"
            :value="formatCurrency(dashboard.netWorthGrowth)"
            :subtitle="`+${formatPercent(dashboard.netWorthGrowthPercent)} จากเดือนที่แล้ว`"
            :is-positive="dashboard.netWorthGrowth > 0"
            variant="primary"
          >
            <template #icon>📈</template>
          </SummaryCard>
        </div>
      </section>

      <!-- Section 2: App Summary with Subscriptions -->
      <section class="section-categories">
        <h2 class="section-title">หมวดหมู่ค่าใช้จ่าย</h2>
        
        <div class="app-summary-full">
          <AppExpenseSummary 
            :categories="dashboard.categories" 
            :subscriptions="dashboard.subscriptions"
            @refresh="loadDashboard"
          />
        </div>
      </section>

      <!-- Section 4: Trackers -->
      <section class="section-trackers">
        <h2 class="section-title">การติดตามรายการ</h2>
        <div class="trackers-grid">
          <InsuranceTracker :insurances="dashboard.insurances" @refresh="loadDashboard" />
          <DebtTracker :debts="dashboard.debts" @refresh="loadDashboard" />
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
import SummaryCard from '../../components/SummaryCard/SummaryCard.vue'
import InsuranceTracker from '../../components/InsuranceTracker/InsuranceTracker.vue'
import DebtTracker from '../../components/DebtTracker/DebtTracker.vue'
import AppExpenseSummary from '../../components/AppExpenseSummary/AppExpenseSummary.vue'
import SummaryEditModal from '../../components/SummaryEditModal/SummaryEditModal.vue'
import ThemeToggle from '../../components/ThemeToggle/ThemeToggle.vue'

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
// Computed Properties
// ============================================================================

// ============================================================================
// Component Functions
// ============================================================================
const loadDashboard = () => refetch()

const handleSaveSummary = async (data: UpdateSummaryRequest) => {
  await updateSummaryMutation.mutateAsync(data)
  showSummaryModal.value = false
}

const handleLogout = () => {
  localStorage.removeItem('token')
  router.push('/')
}

const goToAnalytics = () => {
  router.push('/analytics')
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
