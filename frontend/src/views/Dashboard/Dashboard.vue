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
        <div class="summary-header">
          <h2 class="section-title">Portfolio Overview</h2>
          <button class="btn-edit-summary" @click="showSummaryModal = true" title="Edit summary">
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
