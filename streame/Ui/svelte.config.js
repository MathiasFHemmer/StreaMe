import * as dotenv from 'dotenv';
import adapter from '@sveltejs/adapter-static';
import { vitePreprocess } from '@sveltejs/vite-plugin-svelte';

dotenv.config({ path: process.env.ENV_PATH || '.env.local' });

const customOutput = process.env.OUTPUT_DIR || 'build';

const config = {
	preprocess: vitePreprocess(),

	kit: {
		adapter: adapter({
			pages: customOutput,  
			assets: customOutput,
			fallback: 'index.html'   
		}),
		paths: {
			base: '',
		}
	}
};

export default config;
