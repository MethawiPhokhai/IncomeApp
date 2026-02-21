<template>
  <div v-if="isOpen" class="debt-modal-overlay" @mousedown.self="close">
    <div class="debt-modal">
      <div class="modal-header">
        <h2 class="modal-title">{{ isEditing ? 'แก้ไขรายการผ่อน' : 'เพิ่มรายการผ่อนใหม่' }}</h2>
      </div>
      
      <form @submit.prevent="handleSubmit" class="modal-form">
        <div class="form-group">
          <label class="form-label">ชื่อรายการ</label>
          <input 
            v-model="form.name" 
            type="text" 
            class="form-input" 
            required
            placeholder="เช่น iPhone 15 Pro, ค่าทนาย"
          />
        </div>

        <div class="form-group">
          <label class="form-label">ยอดผ่อนต่อเดือน (บาท)</label>
          <input 
            v-model.number="form.monthlyPayment" 
            type="number" 
            class="form-input" 
            required
            min="0"
            step="0.01"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">งวดปัจจุบัน</label>
            <input 
              v-model.number="form.currentInstallment" 
              type="number" 
              class="form-input" 
              required
              min="0"
            />
          </div>

          <div class="form-group">
            <label class="form-label">จำนวนงวดทั้งหมด</label>
            <input 
              v-model.number="form.totalInstallments" 
              type="number" 
              class="form-input" 
              required
              min="1"
            />
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">ยอดคงเหลือ (บาท)</label>
          <input 
            v-model.number="form.remainingAmount" 
            type="number" 
            class="form-input" 
            required
            min="0"
            step="0.01"
          />
        </div>

        <div class="form-group">
          <label class="form-label">ยอดรวมทั้งหมด (บาท)</label>
          <input 
            v-model.number="form.totalAmount" 
            type="number" 
            class="form-input" 
            required
            min="0"
            step="0.01"
          />
        </div>

        <div class="modal-actions">
          <button type="button" class="btn btn-secondary" @click="close">ยกเลิก</button>
          <button type="submit" class="btn btn-primary">บันทึก</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { ref, watch } from 'vue';
import type { Debt } from '../../services/financialService';

// ============================================================================
// Props & Emits
// ============================================================================
const props = defineProps<{
  isOpen: boolean;
  editItem?: Debt | null;
}>();

const emit = defineEmits(['close', 'save']);

// ============================================================================
// Reactive State
// ============================================================================
const isEditing = ref(false);

const form = ref<Debt>({
  name: '',
  monthlyPayment: 0,
  currentInstallment: 0,
  totalInstallments: 1,
  remainingAmount: 0,
  totalAmount: 0
});

// ============================================================================
// Component Functions
// ============================================================================
const resetForm = () => {
  form.value = {
    name: '',
    monthlyPayment: 0,
    currentInstallment: 0,
    totalInstallments: 1,
    remainingAmount: 0,
    totalAmount: 0
  };
};

const close = () => {
  emit('close');
};

const handleSubmit = () => {
  emit('save', { ...form.value });
  close();
};

// ============================================================================
// Watchers
// ============================================================================
watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    if (props.editItem) {
      isEditing.value = true;
      form.value = { ...props.editItem };
    } else {
      isEditing.value = false;
      resetForm();
    }
  }
});
</script>

<style scoped src="./DebtModal.css"></style>
