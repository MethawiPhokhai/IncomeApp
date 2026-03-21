<template>
  <div class="analytics-page">
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

    <!-- Analytics Content -->
    <div v-else-if="dashboard" class="analytics-content">

      <!-- Page Header -->
      <div class="page-header">
        <div>
          <span class="page-eyebrow">Insights Engine</span>
          <h1 class="page-title">Financial Insights</h1>
        </div>
      </div>

      <!-- Top Bento Grid: 3 cards -->
      <div class="bento-top">
        <!-- Efficiency Score -->
        <div class="card card-score">
          <div class="score-top">
            <div>
              <p class="card-eyebrow">Efficiency Score</p>
              <h3 class="score-num">{{ efficiencyScore }}</h3>
            </div>
            <span class="badge-green">
              <span class="material-icon sm">trending_up</span>
              {{ formatPercent(dashboard.netWorthGrowthPercent) }}
            </span>
          </div>
          <div class="progress-track">
            <div class="progress-fill" :style="{ width: efficiencyScore + '%' }"></div>
          </div>
          <p class="score-desc">
            Saving rate <strong>{{ savingRatePct }}%</strong> of income.
            Net worth growing at {{ formatPercent(dashboard.netWorthGrowthPercent) }}.
          </p>
        </div>

        <!-- Monthly Savings (primary card) -->
        <div class="card card-primary">
          <p class="card-eyebrow-inv">Monthly Savings</p>
          <h3 class="savings-num">{{ formatCurrency(dimeSavings) }}</h3>
          <p class="savings-desc">
            Dime savings this month.
          </p>
        </div>

        <!-- Subscription Burn -->
        <div class="card card-alert">
          <div class="alert-row">
            <div class="alert-icon-wrap">
              <span class="material-icon">warning</span>
            </div>
            <div>
              <p class="alert-title">Active Subscriptions</p>
              <p class="alert-sub">{{ dashboard.subscriptions.length }} recurring payments tracked</p>
            </div>
          </div>
          <p class="alert-body">
            Total <strong>{{ formatCurrency(totalSubscriptionCost) }}/mo</strong> in recurring charges.
            Burn rate {{ formatPercent(dashboard.burnRate) }} of income.
          </p>
          <router-link to="/dashboard" class="alert-link">
            VIEW SUBSCRIPTIONS
            <span class="material-icon sm">arrow_forward</span>
          </router-link>
        </div>
      </div>

      <!-- Middle Grid: Donut + Top Expenses -->
      <div class="bento-middle">
        <!-- Expenses by Category -->
        <div class="card">
          <div class="card-title-row">
            <h4 class="card-title">Expenses by Category</h4>
          </div>
          <div class="donut-layout">
            <!-- SVG Donut -->
            <div class="donut-wrap">
              <svg viewBox="0 0 100 100" class="donut-svg">
                <circle cx="50" cy="50" r="40" fill="transparent" stroke="#EDF1F4" stroke-width="12" />
                <circle
                  v-for="(seg, i) in donutSegments"
                  :key="i"
                  cx="50" cy="50" r="40"
                  fill="transparent"
                  :stroke="seg.color"
                  stroke-width="12"
                  stroke-linecap="butt"
                  :stroke-dasharray="`${seg.arc} ${circumference - seg.arc}`"
                  :stroke-dashoffset="seg.offset"
                  style="transform: rotate(-90deg); transform-origin: center;"
                />
              </svg>
              <div class="donut-center">
                <span class="donut-label">Total</span>
                <span class="donut-total">{{ formatCurrencyShort(totalExpenseChart) }}</span>
              </div>
            </div>
            <!-- Legend -->
            <div class="donut-legend">
              <div v-for="seg in donutSegments" :key="seg.label" class="legend-row">
                <div class="legend-dot" :style="{ background: seg.color }"></div>
                <span class="legend-label">{{ seg.label }}</span>
                <span class="legend-pct">{{ seg.pct }}%</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Top Expenses -->
        <div class="card">
          <div class="card-title-row">
            <div>
              <h4 class="card-title">Top Expenses</h4>
              <p class="card-subtitle">Highest spending categories</p>
            </div>
          </div>
          <div class="vendor-list">
            <div v-for="item in topExpenses" :key="item.label" class="vendor-row">
              <div class="vendor-header">
                <span class="vendor-name">{{ item.label }}</span>
                <span class="vendor-amount">{{ formatCurrency(item.value) }}</span>
              </div>
              <div class="vendor-track">
                <div class="vendor-bar" :style="{ width: item.pct + '%', background: item.color }"></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Subscriptions Table -->
      <section class="card activity-card">
        <div class="table-header">
          <h4 class="card-title">Recurring Subscriptions</h4>
        </div>
        <div class="table-wrap">
          <table class="activity-table">
            <thead>
              <tr>
                <th>Service</th>
                <th>Next Billing</th>
                <th>Cycle</th>
                <th class="text-right">Amount</th>
                <th>Bank</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="sub in dashboard.subscriptions" :key="sub.name">
                <td>
                  <div class="sub-name-cell">
                    <div class="sub-icon">
                      <span class="material-icon sm">subscriptions</span>
                    </div>
                    <div>
                      <span class="sub-name">{{ sub.name }}</span>
                      <span v-if="sub.remark" class="sub-remark">{{ sub.remark }}</span>
                    </div>
                  </div>
                </td>
                <td class="text-muted">{{ formatDate(sub.nextBillingDate) }}</td>
                <td>
                  <span class="badge-cycle">{{ sub.billingCycle }}</span>
                </td>
                <td class="text-right amount-cell">{{ formatCurrency(sub.amount) }}</td>
                <td class="text-muted">{{ sub.bankApp }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { type DashboardSummary } from '../../services/financialService'
import { useApi } from '../../composables/useCrud'
import { formatCurrency, formatPercent } from '../../utils/formatters'

const router = useRouter()

const { data: dashboard, isLoading: loading, error, refetch } = useApi<DashboardSummary>(
  '/api/financial/dashboard'
)

// ── Computed ──────────────────────────────────────────────────────────────────

const savingRatePct = computed(() => {
  if (!dashboard.value) return 0
  const { income, totalSavings, totalInvestment } = dashboard.value
  return income > 0 ? Math.round(((totalSavings + totalInvestment) / income) * 100) : 0
})

const efficiencyScore = computed(() => Math.min(100, Math.round(savingRatePct.value * 1.5 + 40)))

const totalSubscriptionCost = computed(() =>
  dashboard.value?.subscriptions.reduce((sum, s) => sum + s.amount, 0) ?? 0
)

const dimeSavings = computed(() =>
  dashboard.value?.categories
    .filter(c => c.bankApp === 'Dime')
    .reduce((sum, c) => sum + c.amount, 0) ?? 0
)

// ── Donut Chart ────────────────────────────────────────────────────────────────

const circumference = 2 * Math.PI * 40 // r=40 → ≈ 251.3

const totalExpenseChart = computed(() =>
  dashboard.value?.charts.expensesByApp.reduce((s, d) => s + d.value, 0) ?? 0
)

const donutSegments = computed(() => {
  if (!dashboard.value || !totalExpenseChart.value) return []
  let cumulative = 0
  return dashboard.value.charts.expensesByApp.map(item => {
    const pct = parseFloat(((item.value / totalExpenseChart.value) * 100).toFixed(1))
    const arc = (item.value / totalExpenseChart.value) * circumference
    const offset = circumference - cumulative
    cumulative += arc
    return { label: item.label, color: item.color, value: item.value, pct, arc, offset }
  })
})

// ── Top Expenses ───────────────────────────────────────────────────────────────

const topExpenses = computed(() => {
  if (!dashboard.value) return []
  const items = [...dashboard.value.charts.topExpenses].sort((a, b) => b.value - a.value)
  const max = items[0]?.value || 1
  return items.map(item => ({
    ...item,
    pct: Math.round((item.value / max) * 100)
  }))
})

// ── Helpers ────────────────────────────────────────────────────────────────────

const formatCurrencyShort = (val: number) => {
  if (val >= 1_000_000) return `฿${(val / 1_000_000).toFixed(1)}M`
  if (val >= 1_000) return `฿${(val / 1_000).toFixed(1)}k`
  return `฿${val.toFixed(0)}`
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return '-'
  const d = new Date(dateStr)
  return isNaN(d.getTime()) ? dateStr : d.toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })
}

// ── Lifecycle ──────────────────────────────────────────────────────────────────

onMounted(() => {
  if (!localStorage.getItem('token')) router.push('/')
})
</script>

<style scoped src="./Analytics.css"></style>
