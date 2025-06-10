const form = document.getElementById('loginForm');

    form.addEventListener('submit', async (event) => {
      event.preventDefault(); // Evita que se recargue la página

      const username = form.username.value;
      const password = form.password.value;

      try {
        const response = await fetch('https://localhost:5090/api/Security/login', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({ username, password })
        });

        if (!response.ok) {
          // Si el servidor responde con un error HTTP (ej: 401)
          throw new Error('Error en la autenticación');
        }

        const data = await response.json();
        console.log('Respuesta del servidor:', data);

        // Aquí podés hacer algo con la respuesta, ej:
        alert('Login exitoso');

        // Por ejemplo, redireccionar a otra página:
        // window.location.href = '/dashboard';

      } catch (error) {
        console.error('Error:', error);
        alert('Error al iniciar sesión: ' + error.message);
      }
    });