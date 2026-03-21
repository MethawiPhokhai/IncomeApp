<template>
  <div class="app-layout">
    <!-- Mobile Overlay -->
    <div v-if="menuOpen" class="mobile-overlay" @click="menuOpen = false" />

    <!-- Sidebar -->
    <aside class="sidebar" :class="{ 'sidebar-open': menuOpen }">
      <div class="sidebar-brand">
        <h1 class="brand-name">IncomeApp</h1>
        <p class="brand-sub">Expense management</p>
      </div>

      <nav class="sidebar-nav">
        <router-link to="/dashboard" class="nav-item" :class="{ active: route.name === 'dashboard' }" @click="menuOpen = false">
          <span class="nav-icon">pie_chart</span>
          <span>Portfolio</span>
        </router-link>
        <router-link to="/analytics" class="nav-item" :class="{ active: route.name === 'analytics' }" @click="menuOpen = false">
          <span class="nav-icon">analytics</span>
          <span>Analytics</span>
        </router-link>
        <a href="#" class="nav-item disabled">
          <span class="nav-icon">receipt_long</span>
          <span>Transactions</span>
        </a>
        <a href="#" class="nav-item disabled">
          <span class="nav-icon">account_balance_wallet</span>
          <span>Accounts</span>
        </a>
      </nav>

      <div class="sidebar-footer">
        <a href="#" class="nav-item" @click.prevent="handleLogout">
          <span class="nav-icon">logout</span>
          <span>Logout</span>
        </a>
      </div>
    </aside>

    <!-- Main Area -->
    <div class="main-area">
      <!-- Top Header -->
      <header class="top-header">
        <div class="header-left">
          <!-- Hamburger (mobile only) -->
          <button class="hamburger" @click="menuOpen = !menuOpen" aria-label="Menu">
            <span class="nav-icon">{{ menuOpen ? 'close' : 'menu' }}</span>
          </button>
          <div class="header-search">
            <span class="search-icon">search</span>
            <input type="text" placeholder="Search assets..." class="search-input" />
          </div>
        </div>
        <div class="header-actions">
          <ThemeToggle />
        </div>
      </header>

      <!-- Page Content -->
      <main class="page-content">
        <slot />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import ThemeToggle from '../ThemeToggle/ThemeToggle.vue'

const route = useRoute()
const router = useRouter()
const menuOpen = ref(false)

const handleLogout = () => {
  localStorage.removeItem('token')
  router.push('/')
}
</script>

<style scoped src="./AppLayout.css"></style>
