<template>
  <div class="app-expense-summary">
    <div class="section-header">
      <h3 class="section-title">สรุปค่าใช้จ่ายตาม App</h3>
      <button class="btn-add" @click="openAddModal" title="เพิ่มรายการ">
        +
      </button>
    </div>
    
    <div class="app-summary-grid">
      <div 
        v-for="(appData, index) in appSummaries" 
        :key="index"
        class="app-card"
        :class="[`app-${appData.name.toLowerCase()}`, { 'expanded': expandedApp === 'all' || expandedApp === appData.name }]"
        @click.stop="toggleExpand(appData.name)"
      >
        <div class="app-header">
          <div class="app-badge" :class="`badge-${appData.name.toLowerCase()}`">
            {{ appData.name }}
          </div>
          <div class="app-amount">{{ formatCurrency(appData.total) }}</div>
        </div>
        <div class="app-percentage">{{ appData.percentage.toFixed(1) }}% ของยอดรวม</div>
        <div class="app-count">{{ appData.count }} รายการ</div>
        
        <!-- Expandable Details -->
        <div v-if="expandedApp === 'all' || expandedApp === appData.name" class="app-details">
          <div v-for="(item, idx) in appData.items" :key="idx" class="detail-item">
            <div class="item-content">
              <div>
                <span class="item-name">{{ item.name }}</span>
              </div>
              <span class="item-amount">{{ formatCurrency(item.amount) }}</span>
            </div>
            
            <!-- Actions -->
            <div class="item-actions">
              <button class="action-btn btn-edit" @click.stop="openEditModal(item)" title="แก้ไข">
                ✏️
              </button>
              <button class="action-btn btn-delete" @click.stop="confirmDelete(item)" title="ลบ">
                🗑️
              </button>
            </div>
          </div>
        </div>
        
        <div class="expand-indicator">{{ (expandedApp === 'all' || expandedApp === appData.name) ? '▼' : '▶' }}</div>
      </div>
      
      <!-- Subscription Category (Read-Only for now) -->
      <div 
        v-if="subscriptionTotal > 0"
        class="app-card app-subscription"
        :class="{ 'expanded': expandedApp === 'all' || expandedApp === 'Subscription' }"
        @click.stop="toggleExpand('Subscription')"
      >
        <div class="app-header">
          <div class="app-badge badge-subscription">Subscription</div>
          <div class="app-amount">{{ formatCurrency(subscriptionTotal) }}</div>
        </div>
        <div class="app-percentage">{{ subscriptionPercentage.toFixed(1) }}% ของยอดรวม</div>
        <div class="app-count">{{ subscriptions.length }} รายการ</div>
        
        <div v-if="expandedApp === 'all' || expandedApp === 'Subscription'" class="app-details">
          <div v-for="(sub, idx) in subscriptions" :key="idx" class="detail-item">
            <div class="item-content">
              <span class="item-name">
                {{ sub.name }}
                <span v-if="sub.remark" class="item-remark">{{ sub.remark }}</span>
              </span>
              <span class="item-amount">{{ formatCurrency(sub.amount) }}</span>
            </div>
            <!-- No actions for subscriptions yet -->
          </div>
        </div>
        
        <div class="expand-indicator">{{ (expandedApp === 'all' || expandedApp === 'Subscription') ? '▼' : '▶' }}</div>
      </div>
      
      <!-- Other/No App Card -->
      <div 
        v-if="otherTotal > 0" 
        class="app-card app-other"
        :class="{ 'expanded': expandedApp === 'all' || expandedApp === 'Other' }"
        @click.stop="toggleExpand('Other')"
      >
        <div class="app-header">
          <div class="app-badge badge-other">อื่นๆ</div>
          <div class="app-amount">{{ formatCurrency(otherTotal) }}</div>
        </div>
        <div class="app-percentage">{{ otherPercentage.toFixed(1) }}% ของยอดรวม</div>
        <div class="app-count">{{ otherCount }} รายการ</div>
        
        <div v-if="expandedApp === 'all' || expandedApp === 'Other'" class="app-details">
          <div v-for="(item, idx) in otherItems" :key="idx" class="detail-item">
            <div class="item-content">
              <span class="item-name">{{ item.name }}</span>
              <span class="item-amount">{{ formatCurrency(item.amount) }}</span>
            </div>
             <div class="item-actions">
              <button class="action-btn btn-edit" @click.stop="openEditModal(item)" title="แก้ไข">
                ✏️
              </button>
              <button class="action-btn btn-delete" @click.stop="confirmDelete(item)" title="ลบ">
                🗑️
              </button>
            </div>
          </div>
        </div>
        
        <div class="expand-indicator">{{ (expandedApp === 'all' || expandedApp === 'Other') ? '▼' : '▶' }}</div>
      </div>
    </div>
    
    <div class="total-summary">
      <span>ยอดรวมทั้งหมด</span>
      <span class="total-amount">{{ formatCurrency(grandTotal) }}</span>
    </div>

    <ExpenseModal 
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
import { computed, ref } from 'vue';
import { formatCurrency } from '../../utils/formatters';
import { financialService, type CategoryBreakdown, type Subscription } from '../../services/financialService';
import ExpenseModal from '../ExpenseModal/ExpenseModal.vue';

// ============================================================================
// Props & Emits
// ============================================================================
interface Props {
  categories: CategoryBreakdown[];
  subscriptions: Subscription[];
}

const props = defineProps<Props>();
const emit = defineEmits(['refresh']);

// ============================================================================
// Reactive State
// ============================================================================
const expandedApp = ref<string | null>('all');
const showModal = ref(false);
const selectedItem = ref<CategoryBreakdown | null>(null);

// ============================================================================
// Computed Properties
// ============================================================================
const appSummaries = computed(() => {
  const appGroups = new Map<string, { total: number; count: number; items: CategoryBreakdown[] }>();
  
  props.categories.forEach(cat => {
    if (cat.bankApp && cat.bankApp !== 'Subscription') {
      const existing = appGroups.get(cat.bankApp) || { total: 0, count: 0, items: [] };
      existing.total += cat.amount;
      existing.count += 1;
      existing.items.push(cat);
      appGroups.set(cat.bankApp, existing);
    }
  });
  
  return Array.from(appGroups.entries()).map(([name, data]) => ({
    name,
    total: data.total,
    count: data.count,
    items: data.items.sort((a, b) => b.amount - a.amount),
    percentage: grandTotal.value > 0 ? (data.total / grandTotal.value) * 100 : 0
  }));
});

const subscriptionTotal = computed(() => {
  return props.subscriptions.reduce((sum, sub) => sum + sub.amount, 0);
});

const subscriptionPercentage = computed(() => {
  return grandTotal.value > 0 ? (subscriptionTotal.value / grandTotal.value) * 100 : 0;
});

const otherItems = computed(() => {
  return props.categories
    .filter(cat => !cat.bankApp)
    .sort((a, b) => b.amount - a.amount);
});

const otherTotal = computed(() => {
  return otherItems.value.reduce((sum, item) => sum + item.amount, 0);
});

const otherCount = computed(() => {
  return otherItems.value.length;
});

const grandTotal = computed(() => {
  const categoriesTotal = props.categories.reduce((sum, cat) => sum + cat.amount, 0);
  return categoriesTotal + subscriptionTotal.value;
});

const otherPercentage = computed(() => {
  return grandTotal.value > 0 ? (otherTotal.value / grandTotal.value) * 100 : 0;
});

// ============================================================================
// Component Functions
// ============================================================================
const toggleExpand = (appName: string) => {
  if (expandedApp.value === 'all') {
    expandedApp.value = appName;
  } else if (expandedApp.value === appName) {
    expandedApp.value = 'all';
  } else {
    expandedApp.value = appName;
  }
};

const openAddModal = () => {
  selectedItem.value = null;
  showModal.value = true;
};

const openEditModal = (item: CategoryBreakdown) => {
  selectedItem.value = item;
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  selectedItem.value = null;
};

const handleSave = async (item: CategoryBreakdown) => {
  try {
    if (selectedItem.value && selectedItem.value.id) {
      // Edit
      await financialService.updateExpense(selectedItem.value.id, item);
    } else {
      // Add
      await financialService.addExpense(item);
    }
    emit('refresh');
  } catch (error) {
    console.error('Failed to save expense:', error);
    alert('บันทึกไม่สำเร็จ กรุณาลองใหม่อีกครั้ง');
  }
};

const confirmDelete = async (item: CategoryBreakdown) => {
  if (!confirm(`ต้องการลบรายการ "${item.name}" ใช่หรือไม่?`)) return;
  
  try {
    if (item.id) {
      await financialService.deleteExpense(item.id);
      emit('refresh');
    }
  } catch (error) {
    console.error('Failed to delete expense:', error);
    alert('ลบรายการไม่สำเร็จ');
  }
};
</script>

<style scoped src="./AppExpenseSummary.css"></style>
