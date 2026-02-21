<template>
  <div class="chart-container">
    <Pie :data="chartData" :options="chartOptions" />
  </div>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { computed } from 'vue';
import { Pie } from 'vue-chartjs';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import type { ChartDataPoint } from '../../../services/financialService';

// ============================================================================
// Chart.js Registration
// ============================================================================
ChartJS.register(ArcElement, Tooltip, Legend);

// ============================================================================
// Props & Emits
// ============================================================================
interface Props {
  data: ChartDataPoint[];
  title?: string;
}

const props = defineProps<Props>();

// ============================================================================
// Computed Properties
// ============================================================================
const chartData = computed(() => ({
  labels: props.data.map(d => d.label),
  datasets: [{
    data: props.data.map(d => d.value),
    backgroundColor: props.data.map(d => d.color),
    borderWidth: 2,
    borderColor: '#fff'
  }]
}));

// ============================================================================
// Chart Configuration
// ============================================================================
const chartOptions = {
  responsive: true,
  maintainAspectRatio: true,
  plugins: {
    legend: {
      position: 'bottom' as const,
      labels: {
        padding: 15,
        font: {
          size: 12
        }
      }
    },
    tooltip: {
      callbacks: {
        label: function(context: any) {
          const label = context.label || '';
          const value = context.parsed || 0;
          const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0);
          const percentage = ((value / total) * 100).toFixed(1);
          return `${label}: ฿${value.toLocaleString()} (${percentage}%)`;
        }
      }
    }
  }
};
</script>

<style scoped src="./PieChart.css"></style>
