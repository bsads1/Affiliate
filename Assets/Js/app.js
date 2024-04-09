import Inputmask from "inputmask"; 

document.addEventListener('DOMContentLoaded', () => {
    //get all forms
    const forms = document.querySelectorAll('form');
    //check if forms has item in it then disable enter on input
    if (forms.length > 0) {
        forms.forEach(form => {
            form.addEventListener('keydown', event => {
                if (event.key === 'Enter') {
                    event.preventDefault();
                }
            });
        });
    }
});

window.globalCommons = (function () {

    function CreateAlert(message, alertType, target) {
        const alert = `<div role="alert" class="alert alert-${alertType}">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-6 h-6"><path stroke-linecap="round" stroke-linejoin="round" d="m11.25 11.25.041-.02a.75.75 0 0 1 1.063.852l-.708 2.836a.75.75 0 0 0 1.063.853l.041-.021M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Zm-9-3.75h.008v.008H12V8.25Z" /></svg>
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

    function CreateToast(message, alertType, seconds) {
        const alert = `<div class="toast toast-top toast-center toast-invoke"><div class="alert alert-${alertType}"><span>${message}</span></div></div>`;
        //body append this alert and remove it after 3 seconds if seconds is null
        document.body.insertAdjacentHTML('beforeend', alert);
//check seconds null or undefined
        if (seconds === null || seconds === undefined) {
            setTimeout(() => {
                const toast = document.querySelector('.toast-invoke');
                if (toast) {
                    toast.remove();
                }
            }, 3000);
        } else {
            setTimeout(() => {
                const toast = document.querySelector('.toast-invoke');
                if (toast) {
                    toast.remove();
                }
            }, seconds * 1000);
        }
    }

    function AddLoadingToElement(target) {
        const loadingDiv = document.createElement('div');
        loadingDiv.classList.add('loading-overlay');
        loadingDiv.innerHTML = '<span class="loading loading-ring loading-lg"></span>';
        if (target instanceof Element) {
            target.appendChild(loadingDiv);
        } else {
            for (let i = 0; i < target.length; i++) {
                target[i].appendChild(loadingDiv);
            }
        }
    }

    function RemoveLoadingFromElement(target) {
        const loadingDiv = target.querySelector('.loading-overlay');
        if (loadingDiv) {
            if (target instanceof Element) {
                target.removeChild(loadingDiv)
            } else {
                for (let i = 0; i < target.length; i++) {
                    target[i].removeChild(loadingDiv);
                }
            }
        }
    }

    function InputMask(type, options, target) {
        Inputmask(type, options).mask(target);
    }
    
    function ToSlug(text) {
        let slug;

        slug = text.toLowerCase();

        slug = slug.replace(/á|à|ả|ạ|ã|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ/gi, 'a');
        slug = slug.replace(/é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ/gi, 'e');
        slug = slug.replace(/i|í|ì|ỉ|ĩ|ị/gi, 'i');
        slug = slug.replace(/ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ/gi, 'o');
        slug = slug.replace(/ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự/gi, 'u');
        slug = slug.replace(/ý|ỳ|ỷ|ỹ|ỵ/gi, 'y');
        slug = slug.replace(/đ/gi, 'd');

        slug = slug.replace(/\`|\~|\!|\@|\#|\||\$|\%|\^|\&|\*|\(|\)|\+|\=|\,|\.|\/|\?|\>|\<|\'|\"|\:|\;|_/gi, '');

        slug = slug.replace(/ /gi, "-");
        slug = slug.replace(/\-\-\-\-\-/gi, '-');
        slug = slug.replace(/\-\-\-\-/gi, '-');
        slug = slug.replace(/\-\-\-/gi, '-');
        slug = slug.replace(/\-\-/gi, '-');
        slug = '@' + slug + '@';
        slug = slug.replace(/\@\-|\-\@|\@/gi, '');
        return slug;
    }

    return {
        CreateAlert: CreateAlert,
        CreateToast: CreateToast,
        ShowLoading: AddLoadingToElement,
        HideLoading: RemoveLoadingFromElement,
        InputMask: InputMask,
        ToSlug: ToSlug,
        Alert: function () {
            $.alert({
                title: 'Alert!',
                content: 'Simple alert!',
            });
        },
    };
})();