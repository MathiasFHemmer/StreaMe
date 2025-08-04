<script lang="ts">
  import FileUploader from '$lib/FileUploader.svelte';

  // Form state
  let title = '';
  let description = '';
  let releaseYear = '';
  let submitError = '';
  
  // File upload state
  let fileName = '';
  let isUploading = false;
  let uploadComplete = false;
  let uploadProgress = 0;

  const handleFileSelected = (file: File) => {
    fileName = file.name;
    uploadProgress = 0;
  };

  const handleUploadStart = () => {
    isUploading = true;
    uploadComplete = false;
    uploadProgress = 0;
  };

  const handleUploadComplete = () => {
    isUploading = false;
    uploadComplete = true;
    uploadProgress = 100;
  };

  const handleUploadError = (error: Error) => {
    isUploading = false;
    uploadComplete = false;
    console.error('Upload error:', error);
    alert('Upload failed: ' + error.message);
  };

  const handleSubmit = async () => {
    if (!uploadComplete) return;
    
    const formData = {
      title,
      description,
      releaseYear,
      fileName
    };
    
    try {
      const response = await fetch('/add', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          title: fileName, // Using the uploaded filename as title
          path: '',     // Using the form's title field as path
          description,
          releaseYear: Number(releaseYear)
        })
      });

      if (!response.ok) {
        throw new Error(await response.text());
      }

      const result = await response.json();
      console.log('Submission successful:', result);
      alert('Media added successfully!');
    } catch (error) {
      console.error('Submission failed:', error);
      submitError = 'Failed to add media: ' + (error as Error).message;
    }
    
    // Reset form
    title = '';
    description = '';
    releaseYear = '';
    fileName = '';
    uploadComplete = false;
  };
</script>

<div class="form-container">
  <h2>Media Upload Form</h2>

  {#if submitError}
    <div class="error-message">{submitError}</div>
  {/if}
  
  <div class="form-group">
    <label for="title">Title</label>
    <input 
      id="title" 
      type="text" 
      bind:value={title} 
      placeholder="Enter title" 
    />
  </div>
  
  <div class="form-group">
    <label for="description">Description</label>
    <textarea 
      id="description" 
      bind:value={description} 
      placeholder="Enter description"
    ></textarea>
  </div>
  
  <div class="form-group">
    <label for="releaseYear">Release Year</label>
    <input 
      id="releaseYear" 
      type="number" 
      bind:value={releaseYear} 
      placeholder="Enter release year" 
      min="1900" 
      max={new Date().getFullYear()}
    />
  </div>
  
  <div class="form-group">
    <label>File Upload</label>
    <div class="upload-container">
      <FileUploader
        onFileSelected={handleFileSelected}
        onUploadStart={handleUploadStart}
        onUploadComplete={handleUploadComplete}
        onUploadError={handleUploadError}
        onUploadProgress={(progress:number) => uploadProgress = progress}
      />
      
      {#if fileName}
        <span class="file-name">{fileName}</span>
      {/if}
      <!-- Progress bar container -->
      {#if isUploading}
        <div class="progress-container">
          <div 
            class="progress-bar" 
            style={`width: ${uploadProgress}%`}
            role="progressbar"
            aria-valuenow={uploadProgress}
            aria-valuemin="0"
            aria-valuemax="100"
          >
            {uploadProgress}%
          </div>
        </div>
      {:else if uploadComplete}
        <span class="upload-status success">✓ Upload Complete</span>
      {/if}
    </div>
  </div>
  
  <button 
    onclick={handleSubmit} 
    disabled={!uploadComplete}
    class:disabled={!uploadComplete}
  >
    Send
  </button>
</div>

<style>
  .form-container {
    max-width: 500px;
    margin: 0 auto;
    padding: 20px;
    background: #f5f5f5;
    border-radius: 8px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  }

  h2 {
    margin-top: 0;
    color: #333;
  }

  .form-group {
    margin-bottom: 15px;
  }

  label {
    display: block;
    margin-bottom: 5px;
    font-weight: bold;
    color: #555;
  }

  input[type="text"],
  input[type="number"],
  textarea {
    width: 100%;
    padding: 8px;
    border: 1px solid #ddd;
    border-radius: 4px;
    box-sizing: border-box;
  }

  textarea {
    height: 100px;
    resize: vertical;
  }

  .upload-container {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  .file-input {
    display: none;
  }

  .upload-button {
    padding: 8px 12px;
    background: #4CAF50;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    text-align: center;
    transition: background 0.3s;
  }

  .upload-button:hover {
    background: #45a049;
  }

  .file-name {
    font-size: 0.9em;
    color: #666;
  }

  .upload-status {
    font-size: 0.8em;
    color: #666;
  }

  .upload-status.success {
    color: #4CAF50;
    font-weight: bold;
  }

  button {
    width: 100%;
    padding: 10px;
    background: #2196F3;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 16px;
    transition: background 0.3s;
  }

  button:hover:not(:disabled) {
    background: #0b7dda;
  }

  button:disabled {
    background: #cccccc;
    cursor: not-allowed;
    opacity: 0.7;
  }

  .progress-container {
    width: 100%;
    height: 24px;
    background: #e0e0e0;
    border-radius: 4px;
    overflow: hidden;
    margin-top: 8px;
  }
  
  .progress-bar {
    height: 100%;
    background: #4CAF50;
    color: white;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 12px;
    transition: width 0.3s ease;
  }
</style>