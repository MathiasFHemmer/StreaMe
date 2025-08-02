<script lang="ts">
  async function handleFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    try {
      const response = await fetch('/upload', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/octet-stream',
          'X-Filename': encodeURIComponent(file.name)
        },
        body: file.stream(),
        duplex: 'half'
      });

      if (response.ok) {
        console.log('Upload complete');
      } else {
        console.error('Upload failed:', await response.text());
      }
    } catch (err) {
      console.error('Upload error:', err);
    }
  }
</script>

<input type="file" on:change={handleFileSelected} />
