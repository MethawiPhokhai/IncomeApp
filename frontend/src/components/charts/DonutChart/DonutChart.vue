<template>
  <div class="chart-container">
    <Doughnut :data="chartData" :options="chartOptions" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { Doughnut } from 'vue-chartjs';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import type { ChartDataPoint } from '../../../services/financialService';

ChartJS.register(ArcElement, Tooltip, Legend);

interface Props {
  data: ChartDataPoint[];
  title?: string;
}

const props = defineProps<Props>();

const chartData = computed(() => ({
  labels: props.data.map(d => d.label),
  datasets: [{
    data: props.data.map(d => d.value),
    backgroundColor: props.data.map(d => d.color),
    borderWidth: 2,
    borderColor: '#fff'
  }]
}));

const chartOptions = {
  responsive: true,
  maintainAspectRatio: true,
  plugins: {
    legend: {
      position: 'right' as const,
      labels: {
        padding: 10,
        font: {
          size: 11
        },
        usePointStyle: true
      }
    },
    tooltip: {
      callbacks: {
        label: function(context: any) {
          const label = context.label || '';
          const value = context.parsed || 0;
          return `${label}: ฿${value.toLocaleString()}`;
        }
      }
    }
  }
};
</script>

<style scoped src="./DonutChart.css"></style>
