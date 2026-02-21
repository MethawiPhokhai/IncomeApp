<template>
  <div v-if="isOpen" class="expense-modal-overlay" @mousedown.self="close">
    <div class="expense-modal">
      <div class="modal-header">
        <h2 class="modal-title">{{ isEditing ? 'แก้ไขรายการ' : 'เพิ่มรายการใหม่' }}</h2>
      </div>
      
      <form @submit.prevent="handleSubmit" class="modal-form">
        <div class="form-group">
          <label class="form-label">ชื่อรายการ</label>
          <input 
            v-model="form.name" 
            type="text" 
            class="form-input" 
            required
            placeholder="เช่น ค่าอาหาร, ค่าเดินทาง"
          />
        </div>

        <div class="form-group">
          <label class="form-label">จำนวนเงิน</label>
          <input 
            v-model.number="form.amount" 
            type="number" 
            class="form-input" 
            required
            min="0"
            step="0.01"
          />
        </div>

        <div class="form-group">
          <label class="form-label">ประเภท</label>
          <select v-model="form.type" class="form-select" required>
            <option value="Fixed">ค่าใช้จ่ายคงที่ (Fixed)</option>
            <option value="Variable">ค่าใช้จ่ายผันแปร (Variable)</option>
            <option value="Health">สุขภาพ (Health)</option>
            <option value="Family">ครอบครัว (Family)</option>
          </select>
        </div>

        <div class="form-group">
          <label class="form-label">แอปธนาคาร/ที่มา</label>
          <select v-model="form.bankApp" class="form-select" required>
            <option value="Kbank">Kbank</option>
            <option value="KTB">KTB</option>
            <option value="Make">Make by Kbank</option>
            <option value="Dime">Dime</option>
            <option value="Office">Office (หักจากเงินเดือน)</option>
            <option value="Other">อื่นๆ</option>
          </select>
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
import type { CategoryBreakdown } from '../../services/financialService';

// ============================================================================
// Props & Emits
// ============================================================================
const props = defineProps<{
  isOpen: boolean;
  editItem?: CategoryBreakdown | null;
}>();

const emit = defineEmits(['close', 'save']);

// ============================================================================
// Reactive State
// ============================================================================
const isEditing = ref(false);

const form = ref<CategoryBreakdown>({
  name: '',
  amount: 0,
  type: 'Variable',
  color: '',
  bankApp: 'Kbank'
});

// ============================================================================
// Component Functions
// ============================================================================
const resetForm = () => {
  form.value = {
    name: '',
    amount: 0,
    type: 'Variable',
    color: '',
    bankApp: 'Kbank'
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

<style scoped src="./ExpenseModal.css"></style>
