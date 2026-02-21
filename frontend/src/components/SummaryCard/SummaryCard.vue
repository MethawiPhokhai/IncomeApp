<template>
  <div class="summary-card" :class="`summary-card--${variant}`">
    <div class="summary-card__icon">
      <slot name="icon"></slot>
    </div>
    <div class="summary-card__content">
      <div class="summary-card__label">{{ label }}</div>
      <div class="summary-card__value">{{ value }}</div>
      <div v-if="subtitle" class="summary-card__subtitle" :class="{ 'positive': isPositive, 'negative': isNegative }">
        {{ subtitle }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// ============================================================================
// Props & Emits
// ============================================================================
interface Props {
  label: string;
  value: string;
  subtitle?: string;
  variant?: 'primary' | 'success' | 'warning' | 'info';
  isPositive?: boolean;
  isNegative?: boolean;
}

withDefaults(defineProps<Props>(), {
  variant: 'primary',
  isPositive: false,
  isNegative: false
});
</script>

<style scoped src="./SummaryCard.css"></style>
