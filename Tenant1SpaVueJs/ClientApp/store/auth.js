// import { UserManager } from 'oidc-client'
// import config from '../config'

export default {
    state: {
        previousLocation: '/',
        user: {}
    },
    getters: {
    },
    mutations: {
        updatePreviousLocation(state, payload) {
            state.previousLocation = payload
        },
        user(state, payload) {
            state.user = payload.user
        }
    },

    actions: {
    }
}
