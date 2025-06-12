const form = document.getElementById('loginForm');

function getCookie(name) {
  const cookies = document.cookie.split(';');
  for (const cookie of cookies) {
    const [key, value] = cookie.trim().split('=');
    if (key === name) {
      return decodeURIComponent(value);
    }
  }
  return null;
}

form.addEventListener('submit', async (event) => {
  event.preventDefault();

  const username = form.username.value;
  const password = form.password.value;

  const csrfToken = getCookie('XSRF-TOKEN'); 

  try {
    const response = await fetch('https://localhost:8888/api/Security/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-XSRF-TOKEN': csrfToken 
      },
      credentials: 'include', 
      body: JSON.stringify({ username, password })
    });

    if (!response.ok) throw new Error(response.Error.message);

    const data = await response.json();
    console.log('Server response:', data);
    alert(response.value.message);
  } catch (error) {
    console.error('Error:', error);
    alert('Error tp start server: ' + error.message);
  }
});
