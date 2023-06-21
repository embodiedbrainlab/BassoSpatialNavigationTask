// Yanked from http://stackoverflow.com/questions/133925/javascript-post-request-like-a-form-submit

function post(path, params, method) {
	method = method || "post"

	var form = document.createElement("form");
	form.setAttribute("method", method);
	form.setAttribute("action", path);

	for (var key in params){
		if (params.hasOwnProperty(key)) {
			var field = document.createElement("textarea");
			//field.setAttribute("type", "hidden")
			field.setAttribute("name", key)
			field.setAttribute("value", params[key]);

			form.appendChild(field);
		}
	}

	document.body.appendChild(form);
	form.submit();
}