<template>
  <div class="subscription-tracker">
    <h3 class="section-title">รายการ Subscription</h3>
    <div class="subscription-table">
      <div class="table-header">
        <div class="col-name">รายการ</div>
        <div class="col-amount">ยอด</div>
        <div class="col-remark">Remark</div>
      </div>
      <div 
        v-for="(sub, index) in subscriptions" 
        :key="index"
        class="table-row"
      >
        <div class="col-name">
          <span class="sub-name">{{ sub.name }}</span>
        </div>
        <div class="col-amount">
          <span class="amount">{{ formatCurrency(sub.amount) }}</span>
        </div>
        <div class="col-remark">
          <div class="remark-content">
            <span v-if="sub.remark" class="remark-text has-remark">
              {{ sub.remark }}
            </span>
            <span class="bank-badge" v-if="sub.bankApp" :class="`bank-${sub.bankApp.toLowerCase()}`">
              {{ sub.bankApp }}
            </span>
            <span v-if="!sub.remark && !sub.bankApp" class="no-remark">-</span>
          </div>
        </div>
      </div>
    </div>
    <div class="subscription-total">
      <span>รวมทั้งหมด</span>
      <span class="total-amount">{{ formatCurrency(total) }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { formatCurrency } from '../../utils/formatters';
import type { Subscription } from '../../services/financialService';

interface Props {
  subscriptions: Subscription[];
}

const props = defineProps<Props>();

const total = computed(() => 
  props.subscriptions.reduce((sum, sub) => sum + sub.amount, 0)
);
</script>

<style scoped src="./SubscriptionTracker.css"></style>
