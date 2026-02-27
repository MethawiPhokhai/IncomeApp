<template>
  <div class="login-container">
    <!-- Animated Background -->
    <div class="background-glow"></div>
    <div class="background-particles">
      <div class="particle" v-for="i in 20" :key="i" :style="getParticleStyle(i)"></div>
    </div>

    <!-- Login Card -->
    <div class="login-card fade-in">
      <div class="login-header">
        <div class="logo-container">
          <div class="logo-icon">
            <img src="/logo.png" alt="IncomeApp Logo" />
          </div>
        </div>
        <h1 class="text-gradient">Welcome Back</h1>
        <p class="subtitle">Sign in to continue your journey</p>
      </div>

      <div class="login-form">
        <button type="button" @click="handleFacebookLogin" class="btn-facebook" :disabled="isLoading">
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
            <path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z"/>
          </svg>
          Login with Facebook
        </button>

        <div v-if="errorMessage" class="error-message slide-in">
          {{ errorMessage }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

// ============================================================================
// Composables
// ============================================================================
const router = useRouter()

// ============================================================================
// Reactive State
// ============================================================================
const isLoading = ref(false)
const errorMessage = ref('')

// ============================================================================
// Component Functions
// ============================================================================
const getParticleStyle = (_index: number) => {
  const size = Math.random() * 4 + 2
  const duration = Math.random() * 3 + 2
  const delay = Math.random() * 2
  return {
    width: `${size}px`,
    height: `${size}px`,
    left: `${Math.random() * 100}%`,
    top: `${Math.random() * 100}%`,
    animationDuration: `${duration}s`,
    animationDelay: `${delay}s`
  }
}

const initFacebook = () => {
  const appId = import.meta.env.VITE_FACEBOOK_APP_ID;
  if (!appId) {
    console.error('Facebook App ID not found in environment variables');
    return;
  }

  (window as any).fbAsyncInit = function() {
    (window as any).FB.init({
      appId: appId,
      cookie: true,
      xfbml: true,
      version: 'v18.0'
    });
    console.log('Facebook SDK Initialized');
  };

  (function(d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) return;
    js = d.createElement(s) as HTMLScriptElement; 
    js.id = id;
    js.src = "https://connect.facebook.net/en_US/sdk.js";
    if (fjs && fjs.parentNode) {
      fjs.parentNode.insertBefore(js, fjs);
    }
  }(document, 'script', 'facebook-jssdk'));
}

const handleFacebookLogin = async () => {
  isLoading.value = true;
  errorMessage.value = '';

  if (!(window as any).FB) {
    errorMessage.value = 'Facebook SDK not loaded.';
    isLoading.value = false;
    return;
  }

  (window as any).FB.login((response: any) => {
    // Facebook SDK might not like async callbacks, so we wrap the async logic here
    (async () => {
      if (response.authResponse) {
        const accessToken = response.authResponse.accessToken;
        
        try {
          const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5098';
          const apiResponse = await fetch(`${baseUrl}/api/auth/facebook`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({ accessToken })
          });

          const data = await apiResponse.json();

          if (apiResponse.ok && data.success) {
            console.log('Login successful:', data);
            if (data.token) {
              localStorage.setItem('token', data.token);
            }
            // Redirect to dashboard
            router.push('/dashboard');
          } else {
            errorMessage.value = data.message || 'Facebook login failed.';
          }
        } catch (error) {
          errorMessage.value = 'Network error during login.';
          console.error(error);
        }
      } else {
        errorMessage.value = 'User cancelled login or did not fully authorize.';
      }
      isLoading.value = false;
    })();
  }, { scope: 'public_profile,email' });
}

// ============================================================================
// Lifecycle Hooks
// ============================================================================
onMounted(() => {
  initFacebook();
});
</script>

<style scoped src="./Login.css"></style>
