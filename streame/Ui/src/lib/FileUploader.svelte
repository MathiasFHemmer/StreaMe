<script lang="ts">
  let{ input,onFileSelected,onUploadStart,onUploadProgress,onUploadComplete,onUploadError} = $props();

  async function handleFileChange() {
    const file = input.files?.[0];
    if (!file) return;

    onFileSelected?.(file);

    try {
      onUploadStart?.(file);
      const xhr = new XMLHttpRequest();      
      xhr.upload.onprogress = (e) => {
        if (e.lengthComputable) {
          const percent = Math.round((e.loaded / e.total) * 100);
          onUploadProgress?.(percent);
        }
      };

    await new Promise<void>((resolve, reject) => {
      xhr.onload = () => {
        if (xhr.status >= 200 && xhr.status < 300) {
          onUploadComplete?.();
          resolve();
        } else {
          reject(new Error(xhr.statusText));
        }
      };
      xhr.onerror = () => reject(new Error('Upload failed'));
      
      xhr.open('POST', '/upload', true);
      xhr.setRequestHeader('X-Filename', encodeURIComponent(file.name));
      xhr.send(file);
    });

    } catch (err) {
      onUploadError?.(err as Error);
    }
  }
</script>

<input type="file" bind:this={input} on:change={handleFileChange} />