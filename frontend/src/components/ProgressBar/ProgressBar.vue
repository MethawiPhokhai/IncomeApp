<template>
  <div class="progress-bar">
    <div class="progress-bar__header">
      <span class="progress-bar__label">{{ label }}</span>
      <span class="progress-bar__value">{{ current }}/{{ total }}</span>
    </div>
    <div class="progress-bar__track">
      <div 
        class="progress-bar__fill" 
        :style="{ width: `${percentage}%`, backgroundColor: color }"
      ></div>
    </div>
    <div class="progress-bar__info">
      <span>{{ percentage.toFixed(0) }}% เสร็จสิ้น</span>
      <span v-if="remainingAmount">เหลือ {{ formatCurrency(remainingAmount) }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { computed } from 'vue';
import { formatCurrency } from '../../utils/formatters';

// ============================================================================
// Props & Emits
// ============================================================================
interface Props {
  label: string;
  current: number;
  total: number;
  remainingAmount?: number;
  color?: string;
}

const props = withDefaults(defineProps<Props>(), {
  color: '#10b981'
});

// ============================================================================
// Computed Properties
// ============================================================================
const percentage = computed(() => {
  return (props.current / props.total) * 100;
});
</script>

<style scoped src="./ProgressBar.css"></style>
