import {_ask} from "./modules/ask.js";
import {_buttons} from "./modules/buttons.js";

// -- making the javascript available for outside this script
const $ = {
    ask: _ask,
    buttons: _buttons
}
window.$ = $;

// -- check if a player is online
let online = await _ask.online();
if (online) {
    document.getElementById('buttons').classList.remove('d-none');
    _buttons.deploy();
} else {
    document.getElementById('buttons').classList.remove('d-none');
    document.getElementById('buttons').classList.add('d-none');
    if (_ask.get_id() === null || true) {
        // -- show code input
        const code_input = document.getElementById('code_input');
        code_input.classList.remove('d-none');
        document.getElementById('code_input_field_1').focus();
    } else {
        // -- show errors
        if (_ask.get_id() === null) {
            _ask.error('you have not gathered a code.');
        } else {
            _ask.error('you are not online');
        }

    }
}

