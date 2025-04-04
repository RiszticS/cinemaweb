import { ProblemDetails } from "@/api/models/ProblemDetails";
import { HttpError } from "@/api/errors/HttpError";
import { ServerSideValidationError } from "@/api/errors/ServerSideValidationError";

export async function get<TResponse>(url: string, params?: Record<string, string>) : Promise<TResponse> {
    let fullUrl = `${import.meta.env.VITE_APP_API_BASEURL}/${url}`;
    if (params) {
        const queryString = new URLSearchParams(params).toString();
        fullUrl += `?${queryString}`;
    }
    
    const res = await fetch(fullUrl, {
        method: "GET",
    });
    await throwErrorIfNotOk(res);

    return await res.json();
}

export async function postAsJson<TRequest, TResponse>(url: string, body?: TRequest) : Promise<TResponse> {
    const res = await fetch(`${import.meta.env.VITE_APP_API_BASEURL}/${url}`, {
        method: "POST",
        body: body ? JSON.stringify(body) : undefined,
        headers: {
            "Content-Type": "application/json",
        }
    });
    await throwErrorIfNotOk(res);

    return await res.json();
}

export async function postAsJsonWithoutResponse<TRequest>(url: string, body?: TRequest) : Promise<void> {
    const res = await fetch(`${import.meta.env.VITE_APP_API_BASEURL}/${url}`, {
        method: "POST",
        body: body ? JSON.stringify(body) : undefined,
        headers: {
            "Content-Type": "application/json",
        }
    });
    await throwErrorIfNotOk(res);
}

export async function deleteResource(url: string) {
    const res = await fetch(`${import.meta.env.VITE_APP_API_BASEURL}/${url}`, {
        method: "DELETE",
    });
    await throwErrorIfNotOk(res);
}

async function throwErrorIfNotOk(res: Response) {
    if (res.ok) {
        return;
    }
    
    let responseObject;
    try {
        responseObject = await res.json();
    } catch {
        // The response was not json
        throw new HttpError(res.status, `The server returned: ${res.statusText} (${res.status})`);
    }
    
    // The response was not ProblemDetails
    if (!responseObject?.title) {
        throw new HttpError(res.status, `The server returned: ${res.statusText} (${res.status})`);
    }

    // The response was problem ProblemDetails and contains validation errors
    const problemDetails = responseObject as ProblemDetails;
    if (res.status === 400 && problemDetails.errors) {
        // Convert the validation errors from problem details to the format used on the forms
        const validationErrors: Record<string, string> = Object.create(null);
        for (const fieldName in problemDetails.errors) {
            // Make sure that the field names are stored in camelcase
            const fileNameCamelCase = fieldName.charAt(0).toLowerCase() + fieldName.slice(1);
            validationErrors[fileNameCamelCase] = problemDetails.errors[fieldName][0];
        }
        
        throw new ServerSideValidationError(problemDetails.status, problemDetails.title, validationErrors);
    }

    // The response was problem ProblemDetails, check if extended details are provided
    const message = problemDetails.detail
        ? `${problemDetails.title} - ${problemDetails.detail}`
        : problemDetails.title;
    throw new HttpError(problemDetails.status, message);
}