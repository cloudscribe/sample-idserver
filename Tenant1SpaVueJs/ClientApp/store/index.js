import Vue from 'vue'
import Vuex from 'vuex'
import createPersistedState from 'vuex-persistedstate'
Vue.use(Vuex)

import auth from './auth.js'

const store = new Vuex.Store({
    plugins: [createPersistedState({
        storage: window.sessionStorage,
        reducer: state => ({ // so provide here the states you want to persist
            subPath: state.auth.subpath
        })
    })],
    strict: true,
    modules: {
        auth
    }
})

export default store
