<template>
  <div class="debt-tracker">
    <div class="section-header">
      <h3 class="section-title">Recurring Payment</h3>
      <button class="btn-add" @click="openAddModal" title="Add installment">
        <span class="mat-icon">add</span>
      </button>
    </div>
    <div class="debt-list">
      <div v-for="debt in debts" :key="debt.id" class="debt-item">
        <ProgressBar
          :label="debt.name"
          :current="debt.currentInstallment"
          :total="debt.totalInstallments"
          :remaining-amount="debt.remainingAmount"
          :color="getProgressColor(debt.currentInstallment, debt.totalInstallments)"
        >
          <template #actions>
            <div class="item-actions">
              <button class="action-btn btn-edit" @click.stop="openEditModal(debt)" title="Edit">
                <span class="mat-icon">edit</span>
              </button>
              <button class="action-btn btn-delete" @click.stop="confirmDelete(debt)" title="Delete">
                <span class="mat-icon">delete</span>
              </button>
            </div>
          </template>
        </ProgressBar>
        <div class="debt-footer">
          <span class="debt-payment">{{ formatCurrency(debt.monthlyPayment) }}/mo</span>
          <span class="debt-total">Total {{ formatCurrency(debt.totalAmount) }}</span>
        </div>
      </div>
    </div>

    <DebtModal 
      :is-open="showModal" 
      :edit-item="selectedItem" 
      @close="closeModal" 
      @save="handleSave"
    />
  </div>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { ref } from 'vue';
import ProgressBar from '../ProgressBar/ProgressBar.vue';
import { formatCurrency } from '../../utils/formatters';
import { financialService, type Debt } from '../../services/financialService';
import DebtModal from '../DebtModal/DebtModal.vue';

// ============================================================================
// Props & Emits
// ============================================================================
interface Props {
  debts: Debt[];
}

defineProps<Props>();
const emit = defineEmits(['refresh']);

// ============================================================================
// Reactive State
// ============================================================================
const showModal = ref(false);
const selectedItem = ref<Debt | null>(null);

// ============================================================================
// Component Functions
// ============================================================================
const openAddModal = () => {
  selectedItem.value = null;
  showModal.value = true;
};

const openEditModal = (item: Debt) => {
  selectedItem.value = item;
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  selectedItem.value = null;
};

const handleSave = async (item: Debt) => {
  try {
    if (selectedItem.value && selectedItem.value.id) {
      // Edit
      await financialService.updateDebt(selectedItem.value.id, item);
    } else {
      // Add
      await financialService.addDebt(item);
    }
    emit('refresh');
  } catch (error) {
    console.error('Failed to save debt:', error);
    alert('Failed to save. Please try again.');
  }
};

const confirmDelete = async (item: Debt) => {
  if (!confirm(`Delete installment "${item.name}"?`)) return;
  
  try {
    if (item.id) {
      await financialService.deleteDebt(item.id);
      emit('refresh');
    }
  } catch (error) {
    console.error('Failed to delete debt:', error);
    alert('Failed to delete.');
  }
};

const getProgressColor = (current: number, total: number) => {
  const percentage = (current / total) * 100;
  if (percentage >= 75) return '#10b981'; // Green
  if (percentage >= 40) return '#f59e0b'; // Orange
  return '#6366f1'; // Blue
};
</script>

<style scoped src="./DebtTracker.css"></style>
