<template>
  <div class="insurance-tracker">
    <div class="section-header">
      <h3 class="section-title">ปฏิทินจ่ายประกัน</h3>
      <button class="btn-add" @click="openAddModal" title="เพิ่มประกัน">
        +
      </button>
    </div>
    <div class="insurance-list">
      <div 
        v-for="insurance in insurances" 
        :key="insurance.id"
        class="insurance-item"
        :class="`status-${getStatusClass(insurance.dueDate)}`"
      >
        <div class="insurance-info">
          <div class="insurance-provider">{{ insurance.provider }}</div>
          <div class="insurance-policy">{{ insurance.policyName }}</div>
        </div>
        <div class="insurance-details">
          <div class="insurance-amount">{{ formatCurrency(insurance.premium) }}</div>
          <div class="insurance-due">
            <span class="due-label">ครบกำหนด:</span>
            <span class="due-date">{{ formatShortDate(insurance.dueDate) }}</span>
            <span class="due-days">({{ getDaysText(insurance.dueDate) }})</span>
          </div>
        </div>
        
        <!-- Action Buttons -->
        <div class="item-actions">
          <button class="action-btn btn-edit" @click.stop="openEditModal(insurance)" title="แก้ไข">
            ✏️
          </button>
          <button class="action-btn btn-delete" @click.stop="confirmDelete(insurance)" title="ลบ">
            🗑️
          </button>
        </div>
      </div>
    </div>

    <InsuranceModal 
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
import { formatCurrency, formatShortDate, getDaysUntil } from '../../utils/formatters';
import { financialService, type Insurance } from '../../services/financialService';
import InsuranceModal from '../InsuranceModal/InsuranceModal.vue';

// ============================================================================
// Props & Emits
// ============================================================================
interface Props {
  insurances: Insurance[];
}

defineProps<Props>();
const emit = defineEmits(['refresh']);

// ============================================================================
// Reactive State
// ============================================================================
const showModal = ref(false);
const selectedItem = ref<Insurance | null>(null);

// ============================================================================
// Component Functions
// ============================================================================
const openAddModal = () => {
  selectedItem.value = null;
  showModal.value = true;
};

const openEditModal = (item: Insurance) => {
  selectedItem.value = item;
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  selectedItem.value = null;
};

const handleSave = async (item: Insurance) => {
  try {
    if (selectedItem.value && selectedItem.value.id) {
      // Edit
      await financialService.updateInsurance(selectedItem.value.id, item);
    } else {
      // Add
      await financialService.addInsurance(item);
    }
    emit('refresh');
  } catch (error) {
    console.error('Failed to save insurance:', error);
    alert('บันทึกไม่สำเร็จ กรุณาลองใหม่อีกครั้ง');
  }
};

const confirmDelete = async (item: Insurance) => {
  if (!confirm(`ต้องการลบประกัน "${item.provider} - ${item.policyName}" ใช่หรือไม่?`)) return;
  
  try {
    if (item.id) {
      await financialService.deleteInsurance(item.id);
      emit('refresh');
    }
  } catch (error) {
    console.error('Failed to delete insurance:', error);
    alert('ลบรายการไม่สำเร็จ');
  }
};

const getStatusClass = (dueDate: string) => {
  const days = getDaysUntil(dueDate);
  if (days <= 5) return 'urgent';
  if (days <= 10) return 'warning';
  return 'normal';
};

const getDaysText = (dueDate: string) => {
  const days = getDaysUntil(dueDate);
  if (days === 0) return 'วันนี้';
  if (days === 1) return 'พรุ่งนี้';
  if (days < 0) return `เกิน ${Math.abs(days)} วัน`;
  return `อีก ${days} วัน`;
};
</script>

<style scoped src="./InsuranceTracker.css"></style>
