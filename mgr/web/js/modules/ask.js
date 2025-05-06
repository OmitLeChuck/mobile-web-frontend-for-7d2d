class Ask {
    async send(data) {
        data['id'] = data['id'] ?? this.get_id();
        if (data['id'] !== null || data['type'] === 'code') {
            try {
                const requestBody = JSON.stringify(data);
                console.log(requestBody);
                const res = await fetch("/api", {
                    method: "POST",
                    headers: {'Content-Type': 'application/json', "Access-Control-Allow-Origin": "*"},
                    body: requestBody,
                });
                return await res.json() ?? await res.text() ?? {};
            } catch (error) {
                throw error;
            }
        }
    }

    show_menu(toggle = true) {
        if (toggle) {
            document.getElementById('menu').classList.remove('d-none');
        } else {
            document.getElementById('menu').classList.add('d-none');
        }
    }

    async get_position() {
        const position = await this.send({type: 'position'});
        console.log(position);
        return position;
    }

    async set_position() {
        const positionRequest = await this.get_position();
        if (!positionRequest.error) {
            const position = {x: positionRequest.x, z: positionRequest.z, y: positionRequest.y};
            localStorage.setItem('spawnpoint', JSON.stringify(position));
        }

        this.show_menu(false);
    }

    error(message) {
        document.getElementById('error').classList.remove('d-none');
        document.getElementById('error').innerHTML = `<p>${message}</p><p>press <span class="border border-2 p-1 border-white">F5</span> to refresh the page, survivor.</p>`;
    }

    async teleport_to_spawnpoint(target = 'spawnpoint') {
        const position = JSON.parse(localStorage.getItem(target));
        if (position !== null) {
            await this.send({type: 'teleport', x: position.x, z: position.z});
        } else {
            this.error('position not found or empty');
        }
    }

    async process_code() {
        document.getElementById('code_input').classList.add('d-none');
        const code_input = document.getElementById('code_input_field');
        code_input.value = '';
        for (let i = 1; i <= 5; i++) {
            code_input.value += document.getElementById(`code_input_field_${i}`).value.toUpperCase().trim();
        }
        if (code_input.value.length === 5) {
            let request = await this.send({
                type: 'code',
                code: code_input.value.toUpperCase().trim()
            });
            if (request) {
                console.log(request);
                if (!request['error']) {
                    let id = request['id'];
                    if (id !== null) {
                        localStorage.setItem('id', id);
                        document.getElementById('buttons').classList.remove('d-none');
                        location.reload();
                        return;
                    }
                }
            }
        }
        this.error('There is an error with the code-handling you have entered.');
    }

    get_id() {
        return localStorage.getItem('id') ?? null;
    }

    async online() {
        const id = this.get_id();
        if (id !== null) {
            let request = await this.send({type: 'online', id: id});
            console.log(request);
            return request.online;
        }
        return false;
    }

    async bed(data) {
        return await this.send({type: 'bed'});
    }
}

export const _ask = new Ask();