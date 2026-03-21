<script setup lang="ts">
import { useRoute } from 'vue-router'
import AppLayout from './components/AppLayout/AppLayout.vue'

const route = useRoute()
</script>

<template>
  <AppLayout v-if="route.name && route.name !== 'login'">
    <router-view />
  </AppLayout>
  <router-view v-else />
</template>

<style scoped>
</style>
