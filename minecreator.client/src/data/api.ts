import axios from "axios";
import type { Configuration } from "./config";  

export async function GetConfiguration(): Promise<Configuration> {
    const url = "/api/configuration";

    try {
        const res = await axios.get(url, { headers: { Accept: "application/json" } });
        return res.data;
    } catch (err) {
        if (axios.isAxiosError(err)) {
            const status = err.response?.status;
            const statusText = err.response?.statusText || "";
            const data = err.response?.data;
            throw new Error(status ? `${status} ${statusText}: ${JSON.stringify(data)}` : err.message);
        }
        throw err;
    }
}

export async function GenerateOutfits(outfitsExport: any): Promise<any> {
    const url = "/api/generate";

    try {
        const res = await axios.post(url, outfitsExport, { headers: { "Content-Type": "application/json", Accept: "application/json" } });
        return res.data;
    } catch (err) {
        if (axios.isAxiosError(err)) {
            const status = err.response?.status;
            const statusText = err.response?.statusText || "";
            const data = err.response?.data;
            throw new Error(status ? `${status} ${statusText}: ${JSON.stringify(data)}` : err.message);
        }
        throw err;
    }
}
