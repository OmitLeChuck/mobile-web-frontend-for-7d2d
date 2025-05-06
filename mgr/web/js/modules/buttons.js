import {_ask} from "./ask.js";

class Buttons {
    // -- creating a button from a template
    create(id, name, icon, action, element = 'buttons') {

        const button_div = document.getElementById('template_button').cloneNode(true).content.firstElementChild;
        button_div.setAttribute('id', id);
        button_div.addEventListener('click', action);

        const button_icon = button_div.querySelector('#button_icon');
        button_icon.setAttribute('class', icon);

        const button_name = button_div.querySelector('#button_name');
        button_name.innerHTML = name;

        document.getElementById(element).appendChild(button_div);
    }

    // -- deploy al the buttons for the menu and main page
    deploy() {
        // -- main buttons
        [
            {
                id: "bed",
                name: "bed / home",
                icon: "fa-solid fa-bed",
                action: () => _ask.bed()
            },
            {
                id: "teleport_to_spawnpoint",
                name: "teleport to saved spawnpoint",
                icon: "fa-solid fa-bed",
                action: () => _ask.teleport_to_spawnpoint()
            }
        ].forEach(button => {
            this.create(button.id, button.name, button.icon, button.action);
        });

        // -- menu button(s)
        [
            {
                id: "set_spawnpoint",
                name: "set spawnpoint",
                icon: "fa-solid fa-location-pin",
                action: () => _ask.set_position()
            }
        ].forEach(button => {
            this.create(button.id, button.name, button.icon, button.action, 'menu');
        });

        // --- remove template
        document.getElementById('template_button').remove();
    }
}

export const _buttons = new Buttons();