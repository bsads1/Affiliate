document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('UserForm');
    const inputFields = form.querySelectorAll('input');

    inputFields.forEach(input => {
        input.addEventListener('keydown', event => {
            if (event.key === 'Enter') {
                event.preventDefault();
            }
        });
    });
});

window.globalCommons = (function() {
    
    function CreateAlert(message, alertType, target) {
        const alert = `<div role="alert" class="alert alert-${alertType}">
          <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>
        <span>${message}</span>
    </div>`;
        if (target instanceof Element) {
            target.innerHTML = alert;
        } else {
            for (let i = 0; i < target.length; i++) {
                target[i].innerHTML = alert;
            }
        }
    }

    return {
        CreateAlert: CreateAlert
    };
})();