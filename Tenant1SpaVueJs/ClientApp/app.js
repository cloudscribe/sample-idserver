import Vue from 'vue'
import axios from 'axios'
import router from './router'
import store from './store'
import { sync } from 'vuex-router-sync'
import App from 'components/app-root'
import VueAxios from 'vue-axios'
Vue.use(VueAxios, axios)

// Add a request interceptor
axios.interceptors.request.use((config) => {
    // Do something before request is sent
    if (store.state.auth.user.access_token) {
      config.headers['Authorization'] = [store.state.auth.user.token_type, store.state.auth.user.access_token].join(' ')
    }
    else {
      delete config.headers['Authorization']
    }
    return config
  }, (error) => {
    return Promise.reject(error)
  })

sync(store, router)

const app = new Vue({
    store,
    router,
    ...App
})

export {
    app,
    router,
    store
}