import adapter from '@sveltejs/adapter-static';
import { vitePreprocess } from '@sveltejs/vite-plugin-svelte';

const config = {
	preprocess: vitePreprocess(),

	kit: {
		adapter: adapter({
			pages: '../api/wwwroot',  
			assets: '../api/wwwroot',
			fallback: 'index.html'   
		}),
		paths: {
			base: '',
		}
	}
};

export default config;
