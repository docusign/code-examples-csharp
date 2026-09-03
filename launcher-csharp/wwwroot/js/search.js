let DS_SEARCH = (function () {
    const API_TYPES = {
        ESIGNATURE: 'esignature',
        MONITOR: 'monitor',
        CLICK: 'click',
        ROOMS: 'rooms',
        ADMIN: 'admin',
        CONNECT: 'connect',
        WEBFORMS: 'webforms',
        NOTARY: 'notary',
        MAESTRO: 'maestro',
        CONNECTEDFIELDS: 'connectedfields',
        NAVIGATOR: 'navigator',
        WORKSPACES: 'workspaces',
    };

    let processJSONData = function () {
        let json_raw = $("#api_json_data").text()
            , json = json_raw ? JSON.parse(json_raw) : false;

        return json;
    }

    let processCFR11Value = function () {
        let json_raw = $("#cfr11_data").text();

        return json_raw;
    }

    function checkIfExampleMatches(example, matches) {
        const name = example.ExampleName;
        const description = example.ExampleDescription;
        const pathNames = example.LinksToAPIMethod.map(a => a.PathName);

        for (let i = 0; i < matches.length; i++) {
            if (name === matches[i].value || description === matches[i].value || pathNames.indexOf(matches[i].value) > -1) {
                return true;
            }
        }

        return false;
    }

    function clearNonMatchingExamples(examples, matches) {
        for (let i = examples.length - 1; i >= 0; i--) {
            if (!checkIfExampleMatches(examples[i], matches)) {
                examples.splice(i, 1);
            }
        }
    }

    function clearResultsAfterMatching(api, matches) {
        const groups = api.Groups;

        for (let i = groups.length - 1; i >= 0; i--) {
            const group = groups[i];
            clearNonMatchingExamples(group.Examples, matches);

            if (group.Examples.length === 0) {
                groups.splice(i, 1);
            }
        }
    }

    let findCodeExamplesByKeywords = function (json, pattern) {
        const options = {
            isCaseSensitive: false,
            threshold: -0.0,
            includeMatches: true,
            ignoreLocation: true,
            useExtendedSearch: true,
            keys: [
                "Groups.Examples.ExampleName",
                "Groups.Examples.ExampleDescription",
                "Groups.Examples.LinksToAPIMethod.PathName"
            ]
        };

        let clearJSON = JSON.stringify(json).replace(/<\/?[^>]+(>|$)/g, "");
        const fuse = new Fuse(JSON.parse(clearJSON), options);

        var searchResults = fuse.search(JSON.stringify(pattern));



        searchResults.forEach(searchResult => {
            return clearResultsAfterMatching(searchResult.item, searchResult.matches)
        });

        return searchResults;
    }

    let getExamplesByAPIType = function (apiType, codeExamples) {
        let codeExamplesByAPI = codeExamples.find(x => x.Name.toLowerCase() === apiType);

        if (codeExamplesByAPI != null) {
            return [codeExamplesByAPI];
        } else {
            return null;
        }
    }

    let getEnteredAPIType = function (inputValue) {
        const inputLength = inputValue.length;

        for (const key in API_TYPES) {
            if (Object.hasOwnProperty.call(API_TYPES, key)) {
                const apiType = API_TYPES[key];
                const comparedValue = apiType.substr(0, inputLength);

                if (inputValue === comparedValue) {
                    return apiType;
                }
            }
        }

        return null;
    }

    function getSafeUrl(url) {
        const value = String(url);
        // Block script-bearing schemes (javascript:, data:, vbscript:); allow http(s)
        // and relative URLs. encodeURI escapes meta-characters so the value cannot be
        // reinterpreted as markup.
        if (/^\s*(javascript|data|vbscript):/i.test(value)) {
            return "#";
        }

        return encodeURI(value);
    }

    function getLinkForApiType(apiName) {
        switch (apiName) {
            case API_TYPES.ADMIN:
                return "aeg";
            case API_TYPES.CLICK:
                return "ceg";
            case API_TYPES.ROOMS:
                return "reg";
            case API_TYPES.MONITOR:
                return "meg";
            case API_TYPES.ESIGNATURE:
                return "eg";
            case API_TYPES.CONNECT:
                return "con";
            case API_TYPES.MAESTRO:
                return "mae";
            case API_TYPES.WEBFORMS:
                return "web";
            case API_TYPES.NOTARY:
                return "neg";
            case API_TYPES.CONNECTEDFIELDS:
                return "cf";
            case API_TYPES.NAVIGATOR:
                return "nav";
            case API_TYPES.WORKSPACES:
                return "work";
        }
    }

    let addCodeExampleToHomepage = function (codeExamples) {
        var cfrPart11 = processCFR11Value();

        codeExamples.forEach(
            element => {
                let linkToCodeExample = getLinkForApiType(element.Name.toLowerCase());

                element.Groups.forEach(
                    group => {
                        // Build nodes with native DOM APIs (textContent / setAttribute) so that
                        // manifest content is never parsed or reinterpreted as HTML (prevents DOM XSS).
                        const container = document.getElementById("filtered_code_examples");

                        const heading = document.createElement("h2");
                        heading.textContent = group.Name;
                        container.appendChild(heading);

                        group.Examples.forEach(
                            example => {
                                if (!example.SkipForLanguages || !example.SkipForLanguages.toLowerCase().includes("c#")) {
                                    if (element.Name.toLowerCase() !== API_TYPES.ESIGNATURE.toLowerCase() ||
                                        ((example.CFREnabled == "AllAccounts") ||
                                            ((cfrPart11 == "True") && (example.CFREnabled == "CFROnly")) ||
                                            ((cfrPart11 != "True") && (example.CFREnabled == "NonCFR")))) {
                                        let paddedNumber = "0".repeat(3 - example.ExampleNumber.toString().length).concat(example.ExampleNumber);

                                        const exampleLink = document.createElement("a");
                                        exampleLink.setAttribute("href", getSafeUrl(linkToCodeExample.concat(paddedNumber)));
                                        exampleLink.textContent = example.ExampleName;
                                        const exampleHeading = document.createElement("h4");
                                        exampleHeading.setAttribute("id", "example".concat(paddedNumber));
                                        exampleHeading.appendChild(exampleLink);
                                        container.appendChild(exampleHeading);

                                        const description = document.createElement("p");
                                        description.innerHTML = DOMPurify.sanitize(example.ExampleDescription);
                                        container.appendChild(description);

                                        const methodsParagraph = document.createElement("p");
                                        if (example.LinksToAPIMethod.length !== 0) {
                                            if (example.LinksToAPIMethod.length == 1) {
                                                methodsParagraph.appendChild(document.createTextNode(processJSONData().SupportingTexts.APIMethodUsed));
                                            }
                                            else {
                                                methodsParagraph.appendChild(document.createTextNode(processJSONData().SupportingTexts.APIMethodUsedPlural));
                                            }

                                            for (let index = 0; index < example.LinksToAPIMethod.length; index++) {
                                                methodsParagraph.appendChild(document.createTextNode(" "));
                                                const methodLink = document.createElement("a");
                                                methodLink.setAttribute("target", "_blank");
                                                methodLink.setAttribute("href", getSafeUrl(example.LinksToAPIMethod[index].Path));
                                                methodLink.textContent = example.LinksToAPIMethod[index].PathName;
                                                methodsParagraph.appendChild(methodLink);

                                                const separator = document.createElement("span");
                                                if (index + 1 === example.LinksToAPIMethod.length) {
                                                    separator.textContent = "";
                                                }
                                                else if (index + 1 === example.LinksToAPIMethod.length - 1) {
                                                    separator.textContent = " and ";
                                                }
                                                else {
                                                    separator.textContent = ", ";
                                                }
                                                methodsParagraph.appendChild(separator);
                                            }
                                        }

                                        container.appendChild(methodsParagraph);
                                    }
                                }
                            }
                        );
                    }
                );
            }
        );
    }

    let textCouldNotBeFound = function () {
        $("#filtered_code_examples").append(DOMPurify.sanitize(processJSONData().SupportingTexts.SearchFailed));
    }

    return {
        processJSONData: processJSONData,
        getEnteredAPIType: getEnteredAPIType,
        getExamplesByAPIType: getExamplesByAPIType,
        findCodeExamplesByKeywords: findCodeExamplesByKeywords,
        textCouldNotBeFound: textCouldNotBeFound,
        addCodeExampleToHomepage: addCodeExampleToHomepage
    }
})();

const input = document.getElementById('code_example_search');
const log = document.getElementById('values');

input.addEventListener('input', updateValue);

function updateValue(esearchPattern) {
    document.getElementById('filtered_code_examples').innerHTML = "";

    const inputValue = esearchPattern.target.value.toLowerCase();
    const json = DS_SEARCH.processJSONData().APIs;

    if (inputValue === "") {
        DS_SEARCH.addCodeExampleToHomepage(json);
    }
    else {
        const apiType = DS_SEARCH.getEnteredAPIType(inputValue);

        if (apiType !== null) {
            const adminExamples = DS_SEARCH.getExamplesByAPIType(apiType, json);
            DS_SEARCH.addCodeExampleToHomepage(adminExamples);
        }
        else {
            const result = DS_SEARCH.findCodeExamplesByKeywords(json, inputValue);
            if (result.length < 1) {
                DS_SEARCH.textCouldNotBeFound();
            } else {
                result.forEach(x => {
                    var api = json.filter(api => {
                        return api.Name === x.item.Name;
                    })[0];

                    x.item.Groups.forEach((group, groupIndex) => {
                        var unfilteredGroup = api.Groups.filter(apiGroup => {
                            return apiGroup.Name === group.Name;
                        })[0];

                        group.Examples.forEach((example, index) => {
                            var clearedExample = unfilteredGroup.Examples.filter(apiExample => {
                                return apiExample.ExampleNumber === example.ExampleNumber;
                            })[0];
                            x.item.Groups[groupIndex].Examples[index] = clearedExample;
                        });
                    });

                    DS_SEARCH.addCodeExampleToHomepage([x.item]);
                });
            }
        }
    }
}
