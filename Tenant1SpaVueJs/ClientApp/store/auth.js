// import { UserManager } from 'oidc-client'
// import config from '../config'

export default {
    state: {
        subpath: '/',
        user: {}
    },
    getters: {
    },
    mutations: {
        updateSubPath(state, payload) {
            state.subpath = payload
        },
        user(state, payload) {
            state.user = payload.user
        }
    },

    actions: {
    }
}
