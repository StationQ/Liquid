
function ɛ(name, attributes, children) {
    var root = document.createElement("div");
    root.className = "nested-element";

    // XML opening tag
    root.appendChild(document.createTextNode("<"));

    // Element name
    var span = document.createElement("span");
    span.className = "xml-element";
    span.appendChild(document.createTextNode(name));
    root.appendChild(span);

    // Attributes
    if (attributes != null) {
        for (var i = 0; i < attributes.length; i++) {
            if (attributes[i] != undefined) {
                root.appendChild(document.createTextNode(" "));
                root.appendChild(attributes[i]);
            }
        }
    }

    // Children and closing tag
    if (children != null) {
        root.appendChild(document.createTextNode(">"));
        for (var i = 0; i < children.length; i++) {
            if (children[i] != undefined) {
                root.appendChild(children[i]);
            }
        }
        root.appendChild(document.createTextNode("</"));

        // Closing element name
        span = document.createElement("span");
        span.className = "xml-element";
        span.appendChild(document.createTextNode(name));
        root.appendChild(span);

        root.appendChild(document.createTextNode(">"));
    } else {
        root.appendChild(document.createTextNode(" />"));
    }

    return root;
}

function α(name, value) {
    if (name == undefined || value == undefined) {
        return undefined;
    }

    var root = document.createElement("span");

    var span = document.createElement("span");
    span.className = "xml-attribute";
    span.appendChild(document.createTextNode(name));;
    root.appendChild(span);

    root.appendChild(document.createTextNode("=\"" + value + "\""));

    return root;
}

function κ(value) {
    if (value == undefined) {
        return undefined;
    }

    var root = document.createElement("div");
    root.className = "nested-element";

    var span = document.createElement("span");
    span.className = "xml-comment";
    span.appendChild(document.createTextNode("<!-- " + value + " -->"));
    root.appendChild(span);

    return root;
}

function sanitizeTypeNames(typeNamesString) {
    if (typeNamesString === undefined) {
        return undefined;
    }

    // String.replace only replaces the first occurence :/
    typeNamesString = typeNamesString.split("<").join("{");
    typeNamesString = typeNamesString.split(">").join("}");

    return typeNamesString;
}

function t(str) {
    return (str === undefined || str === "") ? undefined : str;
}

function generateRdXml(directives) {
    var root = document.createElement("div");

    var directiveElement;

    isComplete = directives.degreeName && directives.degreeValue;

    if (directives.kind === "type") {
        isComplete = isComplete && t(directives.typeName);
        directiveElement =
            ɛ("Type", [α("Name", sanitizeTypeNames(directives.typeName)), α(directives.degreeName, directives.degreeValue)], null);
    } else if (directives.kind === "namespace") {
        isComplete = isComplete && t(directives.namespaceName);
        directiveElement =
            ɛ("Namespace", [α("Name", directives.namespaceName), α(directives.degreeName, directives.degreeValue)], null);
    } else if (directives.kind === "subtype") {
        isComplete = isComplete && directives.subtypeName;
        directiveElement =
            ɛ("Type", [α("Name", sanitizeTypeNames(directives.subtypeName))],
            [
                ɛ("Subtypes", [α(directives.degreeName, directives.degreeValue)], null)
            ]);
    } else if (directives.kind === "attributeImplies") {
        isComplete = isComplete && t(directives.attributeName);
        directiveElement =
            ɛ("Type", [α("Name", directives.attributeName)],
            [
                ɛ("AttributeImplies", [α(directives.degreeName, directives.degreeValue)], null)
            ]);
    } else if (directives.kind === "assembly") {
        isComplete = isComplete && t(directives.assemblyName);
        directiveElement =
            ɛ("Assembly", [α("Name", directives.assemblyName), α(directives.degreeName, directives.degreeValue)], null);
    } else if (directives.kind === "methodInstantiation") {
        isComplete = t(directives.genericMethodContainingType) && t(directives.genericMethodName) && t(directives.genericMethodArguments);
        directiveElement =
            ɛ("Type", [α("Name", sanitizeTypeNames(directives.genericMethodContainingType))],
            [
                ɛ("MethodInstantiation",
                    [
                        α("Name", directives.genericMethodName),
                        α("Arguments", sanitizeTypeNames(directives.genericMethodArguments)),
                        α("Dynamic", "Required")
                    ], null)
            ]);
    }

    var directiveBody;
    if (isComplete === undefined) {
        directiveBody = [
            κ("WARNING: The directive below is not complete yet!"),
            κ("Answer all questions on the left to finish the snippet"),
            directiveElement];
    } else {
        directiveBody = [directiveElement];
    }

    root.appendChild(ɛ("Directives", [α("xmlns", "http://schemas.microsoft.com/netfx/2013/01/metadata")],
        [
            ɛ("Application", null, directiveBody)
        ]));

    return root;
}

function toggle(id, link) {
    var target = document.getElementById(id);
    if (target.className === "collapsed") {
        link.className = "collapsible";
        target.className = "expanded";
    } else {
        link.className = "expandable";
        target.className = "collapsed";
    }
}


function run() {
    // Prepopulate the generated XML
    var contentElement = document.getElementById("preview");
    contentElement.appendChild(generateRdXml(dataContext));

    // Prevent users from copying the directives before they are complete
    contentElement.addEventListener("selectstart", function () {
        if (!isComplete) {
            window.alert("The Runtime Directive is not complete yet. Answer all questions in the left column before using it.");
            return false;
        }
        return true;
    });

    // Establish one-way data binding for all ms-rd-bind elements
    var elements = document.querySelectorAll("[ms-rd-bind]");
    for (var i = 0; i < elements.length; i++) {
        var eventHandler = {
            element: elements[i],
            handleEvent: function (e) {
                var oldValue;
                var newValue = this.element.value;

                if (this.element.type == "radio") {
                    oldValue = dataContext[this.element.name];
                    dataContext[this.element.name] = newValue;
                } else {
                    oldValue = dataContext[this.element.id];
                    dataContext[this.element.id] = newValue;
                }

                if (oldValue !== newValue) {
                    var contentElement = document.getElementById("preview");
                    contentElement.innerHTML = "";
                    contentElement.appendChild(generateRdXml(dataContext));
                }
            }
        }

        elements[i].addEventListener("change", eventHandler);
        elements[i].addEventListener("paste", eventHandler);
        elements[i].addEventListener("keyup", eventHandler);
    }

    // Set up radio input groups - elements with ms-rd-inputgroup can specify
    // what radio button to check when they are clicked.
    elements = document.querySelectorAll("[ms-rd-inputgroup]");
    for (var i = 0; i < elements.length; i++) {
        var radioGroup = elements[i].getAttribute("ms-rd-inputgroup");
        var radio = document.querySelector('input[value="' + radioGroup + '"]');

        var eventHandler = {
            radioElement: radio,
            handleEvent: function (e) {
                if (!this.radioElement.checked) {
                    this.radioElement.checked = true;

                    // Trigger the change event
                    var event = document.createEvent("HTMLEvents");
                    event.initEvent("change", true, true);
                    this.radioElement.dispatchEvent(event);
                }
            }
        }

        elements[i].addEventListener("click", eventHandler);
    }

    window.addEventListener("scroll", function () {
        var scrollingPart = document.getElementById("scrollingPart");
        var scrollingRect = scrollingPart.getBoundingClientRect();
        var fixedPart = document.getElementById("fixedPart");
        if (scrollingRect.top < 0) {
            var fixedRect = fixedPart.getBoundingClientRect();
            if (scrollingRect.bottom - fixedRect.height < 0) {
                fixedPart.style.top = (scrollingRect.height - fixedRect.height) + "px";
            } else {
                fixedPart.style.top = (-scrollingRect.top) + "px";
            }
        } else if (fixedPart.style.top != "0px") {
            fixedPart.style.top = "0px";
        }
    });
}

var dataContext = new Object();
var isComplete;

window.onload = run;
