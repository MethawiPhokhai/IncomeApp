<template>
  <button 
    class="theme-toggle" 
    @click="toggleTheme" 
    :title="theme === 'light' ? 'Switch to Dark Mode' : 'Switch to Light Mode'"
  >
    <span v-if="theme === 'light'">🌙</span>
    <span v-else>☀️</span>
  </button>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { useTheme } from '../../composables/useTheme';

// ============================================================================
// Composables
// ============================================================================
const { theme, toggleTheme } = useTheme();
</script>

<style scoped>
.theme-toggle {
  background: rgba(255, 255, 255, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: white;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 1.25rem;
  margin-right: 0.5rem;
}

.theme-toggle:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: scale(1.05);
}

/* Logic to adjust icon color if background changes in light/dark mode */
/* But since it's in the header which has a consistent style usually... */
</style>
